using SpikeLib;
using SpikeLib.Messages;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Networking.Sockets;

namespace SpikeComm
{
    class Program
    {


        static async Task Main(string[] args)
        {
            //var portNames = SerialPort.GetPortNames();
            //ManagementClass objInst = new ManagementClass("Win32_SerialPort");
            //var instances = objInst.GetInstances();

            //foreach (var inst in instances)
            //{
            //    Console.WriteLine(inst.ClassPath);
            //    foreach (var prop in inst.Properties)
            //    {
            //        Console.WriteLine(prop.Name + " : " + prop.Value);
            //    }
            //}
            //;

            //var pairedDevices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelector());

            //foreach (var d in pairedDevices)
            //{
            //    if (d.Name.StartsWith("Lego Hub", StringComparison.InvariantCultureIgnoreCase))
            //    {
            //        ;
            //    }
            //    ;
            //}

            //var dd = DeviceInformation.CreateWatcher(BluetoothDevice.GetDeviceSelectorFromPairingState(true));

            //dd.Added += D_Added;

            //// Query for extra properties you want returned
            //string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            //DeviceWatcher deviceWatcher =
            //            DeviceInformation.CreateWatcher(
            //                    BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),
            //                    requestedProperties,
            //                    DeviceInformationKind.AssociationEndpoint);

            //// Register event handlers before starting the watcher.
            //// Added, Updated and Removed are required to get all nearby devices
            //deviceWatcher.Added += DeviceWatcher_Added; 
            //deviceWatcher.Updated += DeviceWatcher_Updated;
            //deviceWatcher.Removed += DeviceWatcher_Removed; ;

            //// Start the watcher.
            //deviceWatcher.Start();

            var ports = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector(), new string[] { "System.DeviceInterface.Serial.PortName" });
            foreach (var port in ports)
            {
                object o = port.Properties["System.DeviceInterface.Serial.PortName"];
            }

            var devices = await SerialSpikeConnection.EnumerateConnectedHubsAsync();

            PipelineTaskMainAsync();

            var watcher = DeviceInformation.CreateWatcher(BluetoothDevice.GetDeviceSelectorFromClassOfDevice(BluetoothClassOfDevice.FromParts(BluetoothMajorClass.Toy, BluetoothMinorClass.ToyRobot, BluetoothServiceCapabilities.None)), new string[]
              {
                "System.ItemNameDisplay",
                "System.Devices.Aep.IsConnected",
                "System.Devices.Aep.DeviceAddress",
                "System.Devices.Aep.ProtocolId",
                "System.Devices.Aep.SignalStrength"
              });

            watcher.Added += Watcher_Added;

            watcher.Removed += Watcher_Removed;
            watcher.Updated += Watcher_Updated;

            watcher.Stopped += Watcher_Stopped;
            watcher.EnumerationCompleted += Watcher_EnumerationCompleted;

            watcher.Start();

            await Task.Delay(100000000);

            ;


        }

        private static CancellationTokenSource tokenSource = new CancellationTokenSource();

        private static  async Task PipelineTaskMainAsync()
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

                            Console.WriteLine(Encoding.UTF8.GetString(line));
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
        }

        private static void Watcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            // watcher.Stop()
            ;
        }

        private static void Watcher_Stopped(DeviceWatcher sender, object args)
        {
            // if watcher.Scanning watcher.Start()
            ;
        }

        private static void Watcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // update if exists in dictionary
            ;
        }

        private static void Watcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // remove from dictionary
            ;
        }

        private static readonly Pipe dataPipe = new Pipe();

        private static async void Watcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            // remove and add to dictionary
            try
            {
                var item = await BluetoothDevice.FromIdAsync(args.Id);

                //item.

                var serialPort = (await item.GetRfcommServicesForIdAsync(RfcommServiceId.SerialPort)).Services.First();
                StreamSocket sock = new StreamSocket();
                await sock.ConnectAsync(serialPort.ConnectionHostName, serialPort.ConnectionServiceName);

                var strm = sock.InputStream.AsStreamForRead();
                while (true)
                {

                    await strm.CopyToAsync(dataPipe.Writer);
                    ;
                }
            }
            catch (Exception ex)
            {
                ;
            }
            ;

            //throw new NotImplementedException();
        }

        public static string MAC802DOT3(ulong macAddress)
        {
            return string.Join(":",
                                BitConverter.GetBytes(macAddress).Reverse()
                                .Select(b => b.ToString("X2"))).Substring(6);
        }

        private static async void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            var ble = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);

            if (ble != null)
            {
                var services = await ble.GetGattServicesAsync();
                var service = services.Services.FirstOrDefault(s => s.Uuid.Equals(new Guid("00001623-1212-efde-1623-785feabcd123")));
                if (service != null)
                {
                    ;
                }
            }
        }

        private static void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            //throw new NotImplementedException();
        }

        private static void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            //throw new NotImplementedException();
        }

        private static void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            //throw new NotImplementedException();
            Console.WriteLine(args.Name);
        }

        private static void D_Added(DeviceWatcher sender, DeviceInformation args)
        {
            ;
        }
    }
}
