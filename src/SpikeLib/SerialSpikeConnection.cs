using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Management;
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

        public static async Task<List<string>> EnumerateConnectedHubsAsync()
        {
            return await Task.Run(() =>
            {
                List<string> ports = new List<string>();
                using ManagementClass objInst = new ManagementClass("Win32_SerialPort");
                var instances = objInst.GetInstances();
                foreach (var item in instances)
                {
                    var pnpDevId = (string)item.GetPropertyValue("PNPDeviceId");
                    if (!pnpDevId.StartsWith("USB", StringComparison.InvariantCultureIgnoreCase)) continue;

                    var vidIndex = pnpDevId.IndexOf("VID_", StringComparison.InvariantCultureIgnoreCase);
                    if (vidIndex < 0) continue;
                    var vid = pnpDevId.Substring(vidIndex + 4, 4);
                    if (vid.Equals("0694", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ports.Add((string)item.GetPropertyValue("DeviceId"));
                    }
                }
                return ports;
            });
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
