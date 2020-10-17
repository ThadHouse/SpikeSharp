using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json;
using SpikeLib.Messages;
using StreamJsonRpc;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.IO.Ports;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace SpikeLib
{

    public class SpikeHub
    {
        public string Port { get; }
        private SerialPort serialPort;
        Pipe dataPipe = new Pipe();
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        public SpikeHub(string comPort)
        {
            Port = comPort;
            serialPort = new SerialPort(comPort, 115200);
        }

        private Task? RunLoop;

        public async Task OpenAsync()
        {
            await Task.Run(() =>
            {
                serialPort.RtsEnable = true;
                serialPort.DataBits = 8;
                serialPort.StopBits = StopBits.One;
                serialPort.Parity = Parity.None;
                serialPort.DtrEnable = true;
                serialPort.Open();
                tokenSource = new CancellationTokenSource();
                RunLoop = Task.Run(() => ThreadMainAsync());
            });
        }

        public async Task CloseAsync()
        {
            tokenSource.Cancel();
            if (RunLoop != null)
            {
                await RunLoop;
                RunLoop = null;
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
            serialPort.Close();
        }

        

        bool isFirstLine = true;
        byte[] mChar = new byte[] { (byte)'m' };
        byte[] pChar = new byte[] { (byte)'p' };

        public async Task<IMessage> ReadMessageAsync()
        {

            var reader = dataPipe.Reader;
            IMessage? toRet = null;
            while (true)
            {
                ReadResult result = await reader.ReadAsync();

                ReadOnlySequence<byte> buffer = result.Buffer;
                SequencePosition? position = null;


                // Look for a EOL in the buffer
                position = buffer.PositionOf((byte)'\r');

                if (position != null)
                {
                    // Process the line
                    ReadOnlySequence<byte> line = buffer.Slice(0, position.Value);
                    if (isFirstLine)
                    {
                        isFirstLine = false;
                    }
                    else
                    {
                        using var document = JsonDocument.Parse(line);
                        toRet = IMessage.ParseMessage(document);
                    }

                    // Skip the line + the \n character (basically position)
                    buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
                }

                // Tell the PipeReader how much of the buffer we have consumed
                reader.AdvanceTo(buffer.Start, buffer.End);

                if (toRet != null)
                {
                    return toRet;
                }
            }
        }
    }
}
