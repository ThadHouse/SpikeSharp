using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json;
using SpikeLib.Messages;
using SpikeLib.Responses;
using StreamJsonRpc;
using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.IO.Ports;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SpikeLib
{

    public class SpikeHub
    {
        public string Port { get; }
        private readonly SerialPort serialPort;
        private readonly Pipe dataPipe = new Pipe();
        private readonly Channel<IMessage> unknownMessagesChannel;
        private readonly Channel<StorageResponse> storageResponseChannel;
        private readonly Channel<IConsoleMessage> consoleMessagesChannel;

        public ChannelReader<IMessage> UnknownMessagesReader => unknownMessagesChannel.Reader;
        public ChannelReader<IConsoleMessage> ConsoleMessagesReader => consoleMessagesChannel.Reader;

        public SpikeHub(string comPort)
        {
            Port = comPort;
            serialPort = new SerialPort(comPort, 115200);
            unknownMessagesChannel = Channel.CreateUnbounded<IMessage>();
            storageResponseChannel = Channel.CreateUnbounded<StorageResponse>();
            consoleMessagesChannel = Channel.CreateUnbounded<IConsoleMessage>();
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
                var memory = pipe.GetMemory(2048);
                int readBytes = await stream.ReadAsync(memory, token);
                if (readBytes < 0)
                {
                    await pipe.CompleteAsync();
                } 
                pipe.Advance(readBytes);
                await pipe.FlushAsync(token);
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
                    catch (Exception ex)
                    {
                        // TODO handle parsing exception
                    }
                    buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
                }

                reader.AdvanceTo(buffer.Start, buffer.End);

            }
            unknownMessagesChannel.Writer.Complete();
            storageResponseChannel.Writer.Complete();
            consoleMessagesChannel.Writer.Complete();
        }

        private async ValueTask HandleMessageAsync(IMessage message, CancellationToken token)
        {
            if (message is StorageResponse storage)
            {
                await storageResponseChannel.Writer.WriteAsync(storage, token);
            }
            else
            {
                await unknownMessagesChannel.Writer.WriteAsync(message, token);
            }
        }

        public async Task<StorageResponse> RequestStorageAsync(CancellationToken cancellationToken = default)
        {
            string randomString;
            using var stream = new MemoryStream();
            {
                using var writer = new Utf8JsonWriter(stream);
                writer.WriteStartObject();
                writer.WriteString("m", "get_storage_status");
                writer.WriteStartObject("p");
                writer.WriteEndObject();

                randomString = "0abc";

                writer.WriteString("i", randomString);
                writer.WriteEndObject();
            }
            stream.WriteByte(13);
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(serialPort.BaseStream, cancellationToken);

            return await storageResponseChannel.Reader.ReadAsync(cancellationToken);
        }
    }
}
