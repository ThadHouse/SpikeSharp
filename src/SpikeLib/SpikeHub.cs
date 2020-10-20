using SpikeLib.Messages;
using SpikeLib.Responses;
using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.IO.Ports;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SpikeLib
{

    public class SpikeHub : IDisposable
    {
        public string Port { get; }
        private readonly SerialPort serialPort;
        private readonly Pipe dataPipe = new Pipe();
        private readonly Channel<IMessage> unknownMessagesChannel;
        private readonly Channel<StorageResponse> storageResponseChannel;
        private readonly Channel<IConsoleMessage> consoleMessagesChannel;
        private readonly Channel<IResponse> programWriteChannel;
        private readonly Channel<IStatusMessage> statusMessageChannel;
        private readonly Channel<StorageResponse> storageUpdateChannel;

        public ChannelReader<IMessage> UnknownMessagesReader => unknownMessagesChannel.Reader;
        public ChannelReader<IConsoleMessage> ConsoleMessagesReader => consoleMessagesChannel.Reader;
        public ChannelReader<IStatusMessage> StatusMessageReader => statusMessageChannel.Reader;
        public ChannelReader<StorageResponse> StorageUpdateReader => storageUpdateChannel.Reader;

        public SpikeHub(string comPort)
        {
            Port = comPort;
            serialPort = new SerialPort(comPort, 115200);
            unknownMessagesChannel = Channel.CreateUnbounded<IMessage>();
            storageResponseChannel = Channel.CreateUnbounded<StorageResponse>();
            consoleMessagesChannel = Channel.CreateUnbounded<IConsoleMessage>();
            programWriteChannel = Channel.CreateUnbounded<IResponse>();
            statusMessageChannel = Channel.CreateUnbounded<IStatusMessage>();
            storageUpdateChannel = Channel.CreateUnbounded<StorageResponse>();
        }

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        private Task? SerialReadLoopLoop;
        private Task? PipelineReadLoop;

        public async Task OpenAsync()
        {
            if (serialPort.IsOpen) return;
            await Task.Run(() =>
            {
                serialPort.RtsEnable = true;
                serialPort.DataBits = 8;
                serialPort.StopBits = StopBits.One;
                serialPort.Parity = Parity.None;
                serialPort.DtrEnable = true;
                serialPort.Open();
                tokenSource = new CancellationTokenSource();
                SerialReadLoopLoop = Task.Run(() => ThreadMainAsync());
                PipelineReadLoop = Task.Run(() => PipelineTaskMainAsync());
            });
        }

        public async Task CloseAsync()
        {
            tokenSource.Cancel();
            if (SerialReadLoopLoop != null)
            {
                await SerialReadLoopLoop;
                SerialReadLoopLoop = null;
            }
            if (PipelineReadLoop != null)
            {
                await PipelineReadLoop;
                PipelineReadLoop = null;
            }
        }

        private async Task ThreadMainAsync()
        {
            var token = tokenSource.Token;
            var stream = serialPort.BaseStream;
            stream.ReadTimeout = 100;
            var pipe = dataPipe.Writer;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await stream.CopyToAsync(pipe, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            await pipe.CompleteAsync();
            serialPort.Close();
        }

        private async Task PipelineTaskMainAsync()
        {
            var reader = dataPipe.Reader;
            var token = tokenSource.Token;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    ReadResult result = await reader.ReadAsync(token);
                    if (result.IsCanceled || result.IsCompleted)
                    {
                        break;
                    }

                    var buffer = result.Buffer;

                    var position = buffer.PositionOf<byte>(13);

                    if (position != null)
                    {
                        try
                        {
                            ReadOnlySequence<byte> line = buffer.Slice(0, position.Value);

                            using var document = JsonDocument.Parse(line);
                            var parsedMessage = IMessage.ParseMessage(document);
                            if (parsedMessage != null)
                            {
                                await HandleMessageAsync(parsedMessage, token);
                            }
                        }
#pragma warning disable CA1031 // Do not catch general exception types
                        catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
                        {
                            // TODO handle parsing exception
                        }
                        buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
                    }

                    reader.AdvanceTo(buffer.Start, buffer.End);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            unknownMessagesChannel.Writer.Complete();
            storageResponseChannel.Writer.Complete();
            consoleMessagesChannel.Writer.Complete();
            programWriteChannel.Writer.Complete();
            statusMessageChannel.Writer.Complete();
            storageUpdateChannel.Writer.Complete();
        }

        private async ValueTask HandleMessageAsync(IMessage message, CancellationToken token)
        {
            if (message is StorageResponse storage)
            {
                if (storage.Id == "STATUS")
                {
                    await storageUpdateChannel.Writer.WriteAsync(storage, token);
                }
                else
                {
                    await storageResponseChannel.Writer.WriteAsync(storage, token);
                }
            }
            else if (message is StartWriteProgramResponse startResponse)
            {
                await programWriteChannel.Writer.WriteAsync(startResponse, token);
            }
            else if (message is WritePackageResponse writePackage)
            {
                await programWriteChannel.Writer.WriteAsync(writePackage, token);
            }
            else if (message is IStatusMessage statusMessage)
            {
                await statusMessageChannel.Writer.WriteAsync(statusMessage, token);
            }
            else if (message is IConsoleMessage consoleMessage)
            {
                await consoleMessagesChannel.Writer.WriteAsync(consoleMessage, token);
            }
            else
            {
                await unknownMessagesChannel.Writer.WriteAsync(message, token);
            }
        }

        public async Task<StorageResponse> RequestStorageAsync(CancellationToken cancellationToken = default)
        {
            using var stream = new MemoryStream();
            {
                using var writer = new Utf8JsonWriter(stream);
                writer.WriteStartObject();
                writer.WriteString("m", "get_storage_status");
                writer.WriteStartObject("p");
                writer.WriteEndObject();

                string randomString = "0abc";

                writer.WriteString("i", randomString);
                writer.WriteEndObject();
            }
            stream.WriteByte(13);
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(serialPort.BaseStream, cancellationToken);

            return await storageResponseChannel.Reader.ReadAsync(cancellationToken);
        }

        public async Task<bool> UploadFileAsync(Stream fileToUpload, int slot, string name, CancellationToken cancellationToken = default)
        {
            if (fileToUpload == null)
            {
                throw new ArgumentNullException(nameof(fileToUpload));
            }

            using var postStream = new MemoryStream();
            {
                using var writer = new Utf8JsonWriter(postStream);
                writer.WriteStartObject();
                var nowTime = DateTime.UtcNow.Ticks;
                writer.WriteString("m", "start_write_program");

                writer.WriteStartObject("p");
                writer.WriteNumber("slotid", slot);
                writer.WriteNumber("size", fileToUpload.Length);

                writer.WriteStartObject("meta");
                writer.WriteNumber("created", nowTime);
                writer.WriteNumber("modified", nowTime);
                writer.WriteBase64String("name", Encoding.UTF8.GetBytes(name));
                writer.WriteString("type", "python");
                writer.WriteString("project_id", "HelloWorld12");
                writer.WriteEndObject();

                writer.WriteEndObject();
                string randomString = "1abc";

                writer.WriteString("i", randomString);
                writer.WriteEndObject();
            }

            postStream.WriteByte(13);
            postStream.Seek(0, SeekOrigin.Begin);
            await postStream.CopyToAsync(serialPort.BaseStream, cancellationToken);

            var readValue = await programWriteChannel.Reader.ReadAsync(cancellationToken);

            var startResponse = (StartWriteProgramResponse)readValue;

            Memory<byte> memoryBuffer = new byte[startResponse.BlockSize];

            do
            {
                var readBytes = await fileToUpload.ReadAsync(memoryBuffer, cancellationToken);
                postStream.Seek(0, SeekOrigin.Begin);
                postStream.SetLength(0);

                {
                    using var writer = new Utf8JsonWriter(postStream);
                    writer.WriteStartObject();
                    writer.WriteString("m", "write_package");

                    writer.WriteStartObject("p");
                    writer.WriteBase64String("data", memoryBuffer.Span.Slice(0, readBytes));
                    writer.WriteString("transferid", startResponse.TransferId);
                    writer.WriteEndObject();

                    string randomString = "2abc";

                    writer.WriteString("i", randomString);
                    writer.WriteEndObject();
                }

                postStream.WriteByte(13);
                postStream.Seek(0, SeekOrigin.Begin);
                await postStream.CopyToAsync(serialPort.BaseStream, cancellationToken);

                var readWriteResponse = await programWriteChannel.Reader.ReadAsync(cancellationToken);
                ;

            }
            while (fileToUpload.Position < fileToUpload.Length);

            return true;
        }

        public void Dispose()
        {
            serialPort?.Dispose();
            tokenSource?.Dispose();
        }
    }
}
