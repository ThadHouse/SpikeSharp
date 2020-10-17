using Newtonsoft.Json;
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
        Thread? dataThread;
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        JsonRpc jsonRpc = null!;

        public SpikeHub(string comPort)
        {
            Port = comPort;
            serialPort = new SerialPort(comPort, 115200);
        }

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
                //jsonRpc = JsonRpc.Attach(serialPort.BaseStream);
                dataThread = new Thread(ThreadMain);
                dataThread.Start();
            });
        }



        private void ThreadMain()
        {
            var token = tokenSource.Token;
            serialPort.ReadTimeout = 500;
            Span<byte> singleByte = stackalloc byte[1];
            while (!token.IsCancellationRequested)
            {
                try
                {
                    int value = serialPort.ReadByte();
                    if (value < 0)
                    {
                        dataPipe.Writer.Complete();
                        break;
                    }
                    singleByte[0] = (byte)value;
                    dataPipe.Writer.Write(singleByte);
                    if (value == '\r')
                    {
                        dataPipe.Writer.FlushAsync().AsTask().Wait();
                    }
                }
                catch (TimeoutException)
                {
                    int numBytes = serialPort.BytesToRead;
                    continue;
                }
            }
        }

        bool isFirstLine = true;
        byte[] mChar = new byte[] { (byte)'m' };

        public async Task<string?> ReadLine()
        {

            var reader = dataPipe.Reader;
            string? toRet = null;
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
                        var methodId = document.RootElement.GetProperty(mChar).GetInt32();
                        toRet = methodId.ToString();

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

                // Stop reading if there's no more data coming
                if (result.IsCompleted)
                {
                    return null;
                }


            }
        }
    }
}
