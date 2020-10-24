using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;

namespace SpikeLib
{

    public class SerialSpikeConnection : ISpikeConnection
    {
        public Stream ReadStream { get; }

        public Stream WriteStream { get; }

        private readonly SerialPort serialPort;

        public SerialSpikeConnection(SerialPort serialPort)
        {
            if (serialPort == null)
            {
                throw new ArgumentNullException(nameof(serialPort));
            }

            if (!serialPort.IsOpen)
            {
                throw new InvalidOperationException("Port must be opened");
            }

            ReadStream = serialPort.BaseStream;
            WriteStream = serialPort.BaseStream;
            this.serialPort = serialPort;
            serialPort.ErrorReceived += SerialPort_ErrorReceived;
        }

        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            ;
        }

        public async Task CloseAsync()
        {
            await Task.Run(() => serialPort.Close());
        }

        public static async Task<SerialSpikeConnection?> OpenConnectionAsync(string comPort)
        {
            if (string.IsNullOrWhiteSpace(comPort))
            {
                throw new ArgumentOutOfRangeException(nameof(comPort), "Port string must not be null or empty");
            }
            return await Task.Run(() =>
            {
                try
                {
                    var port = new SerialPort(comPort)
                    {
                        RtsEnable = true,
                        DtrEnable = true,
                    };
                    port.Open();

                    return new SerialSpikeConnection(port);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    return null;
                }
            });
        }
    }
}
