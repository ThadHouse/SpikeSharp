using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SpikeApp.Utilities;
using SpikeLib;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Networking.Sockets;

namespace SpikeApp.Controls.ViewModels
{
    public class SpikePortControlViewModel : ViewModelBase
    {
        public class HubInfo
        {
            public DeviceInformation DeviceInfo { get; }

            public HubInfo(DeviceInformation info)
            {
                DeviceInfo = info;
            }

            public override string ToString()
            {
                return Name;
            }

            public string Id => DeviceInfo.Id;
            public string Name => DeviceInfo.Name;
            public bool IsSerial => DeviceInfo.Kind == DeviceInformationKind.DeviceInterface;
            public bool IsValid { get; set; }
        }

        public ObservableCollection<HubInfo> Devices { get; } = new ObservableCollection<HubInfo>();

        private readonly DeviceWatcher deviceWatcherSerial;
        private readonly DeviceWatcher deviceWatcherBluetooth;
        private readonly SynchronizationContext context;
        private readonly SendOrPostCallback sendOrPostCb;

        private record UpdateWrapper(bool Removed, DeviceInformationUpdate Update);

        private HubInfo? connectedDevice;

        private async void TriggerRefresh(object? o)
        {
            if (o is DeviceInformation devInfo)
            {
                Devices.Add(new HubInfo(devInfo));
                await RefreshAsync();
                ;
            }
            else if (o is UpdateWrapper updateWrapper)
            {
                if (updateWrapper.Removed)
                {
                    // Find device
                    for (int i = 0; i < Devices.Count; i++)
                    {
                        if (Devices[i].Id == updateWrapper.Update.Id)
                        {
                            var device = Devices[i];
                            Devices.RemoveAt(i);
                            if (device == connectedDevice)
                            {
                                // Disconnect
                                await DisconnectAsync();
                                if (SelectedDevice == device)
                                {
                                    SelectedDevice = null;
                                }
                            }
                            // Refresh
                            await RefreshAsync();

                            break;
                        }
                    }
                }
                else
                {
                    foreach (var device in Devices)
                    {
                        if (device.Id == updateWrapper.Update.Id)
                        {
                            device.DeviceInfo.Update(updateWrapper.Update);
                            break;
                        }
                    }
                }
            }
        }

        public SpikePortControlViewModel()
        {
            sendOrPostCb = TriggerRefresh;
            context = SynchronizationContext.Current!;
            var deviceClass = BluetoothDevice.GetDeviceSelectorFromClassOfDevice(BluetoothClassOfDevice.FromParts(BluetoothMajorClass.Toy, BluetoothMinorClass.ToyRobot, BluetoothServiceCapabilities.None));
            deviceWatcherBluetooth = DeviceInformation.CreateWatcher(deviceClass);
            deviceWatcherSerial = DeviceInformation.CreateWatcher(SerialDevice.GetDeviceSelectorFromUsbVidPid(0x0694, 0x0010), new string[] { "System.DeviceInterface.Serial.PortName" });

            deviceWatcherSerial.Added += DeviceWatcher_Added;
            deviceWatcherSerial.Removed += DeviceWatcher_Removed;
            deviceWatcherSerial.Updated += DeviceWatcher_Updated;
            deviceWatcherSerial.Stopped += DeviceWatcher_Stopped;
            deviceWatcherSerial.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;

            deviceWatcherBluetooth.Added += DeviceWatcher_Added;
            deviceWatcherBluetooth.Removed += DeviceWatcher_Removed;
            deviceWatcherBluetooth.Updated += DeviceWatcher_Updated;
            deviceWatcherBluetooth.Stopped += DeviceWatcher_Stopped;
            deviceWatcherBluetooth.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;

            deviceWatcherSerial.Start();
            deviceWatcherBluetooth.Start();
        }

        private bool canStartConnect = false;
        public bool CanStartConnect
        {
            get => canStartConnect;
            set => RaiseAndSetIfChanged(ref canStartConnect, value);
        }

        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            ;
        }

        private void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            // TODO only do if we actually want it running
            sender.Start();
        }

        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            context.Post(sendOrPostCb, new UpdateWrapper(false, args));
            ;
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            context.Post(sendOrPostCb, new UpdateWrapper(true, args));
        }

        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            if (sender == deviceWatcherBluetooth)
            {
                if (!args.Name.StartsWith("LEGO Hub", StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }
            }
            context.Post(sendOrPostCb, args);
        }

        private HubInfo? selectedDevice;
            
        public HubInfo? SelectedDevice
        {
            get => selectedDevice;
            set => RaiseAndSetIfChanged(ref selectedDevice, value);
        }

        private string connectText = "Connect";

        public string ConnectText
        {
            get => connectText;
            set => RaiseAndSetIfChanged(ref connectText, value);
        }

        private bool isConnected;

        public bool IsConnected
        {
            get => isConnected;
            set => RaiseAndSetIfChanged(ref isConnected, value);
        }

        public async Task ConnectAsync(HubInfo device)
        {
            try
            {
                CanStartConnect = false;
                if (device.IsSerial)
                {
                    var props = device.DeviceInfo.Properties.ToArray();
                    foreach (var prop in props)
                    {
                        if (prop.Key == "System.DeviceInterface.Serial.PortName")
                        {
                            var conn = await SerialSpikeConnection.OpenConnectionAsync((string)prop.Value);
                            if (conn != null)
                            {
                                await ViewModelStorage.AddHubAsync(conn);
                                connectedDevice = device;
                                IsConnected = true;
                                ConnectText = "Disconnect";
                            }

                        }
                    }
                }
                else
                {
                    try
                    {
                        var btDevice = await BluetoothDevice.FromIdAsync(device.Id);

                        var serialPort = (await btDevice.GetRfcommServicesForIdAsync(RfcommServiceId.SerialPort)).Services.First();
                        StreamSocket streamSocket = new();
                        await streamSocket.ConnectAsync(serialPort.ConnectionHostName, serialPort.ConnectionServiceName);

                        var conn = new StreamSpikeConnection(streamSocket);

                        await ViewModelStorage.AddHubAsync(conn);
                        connectedDevice = device;
                        IsConnected = true;
                        ConnectText = "Disconnect";
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        ;
                    }
                }
            ;
            }
            finally
            {
                CanStartConnect = true;
            }
        }

        public async Task RefreshAsync()
        {
            if (connectedDevice != null) return;

            // Enumerate, see if we have a serial connection
            foreach (var device in Devices)
            {
                if (device.IsSerial)
                {
                    SelectedDevice = device;
                    await ConnectAsync(device);
                    break;
                }
            }

            if (Devices.Count == 0)
            {
                CanStartConnect = false;
            }
            else
            {
                CanStartConnect = true;
                if (SelectedDevice == null)
                {
                    SelectedDevice = Devices[0];
                }
            }
        }

        public async Task DisconnectAsync()
        {
            if (connectedDevice == null) return;

            try
            {
                CanStartConnect = false;
                await ViewModelStorage.CloseHubAsync();
                connectedDevice = null;
                IsConnected = false;
                ConnectText = "Connect";
            }
            finally
            {
                CanStartConnect = true;
            }
        }

        public async void Connect()
        {
            if (IsConnected)
            {
                CanStartConnect = false;
                await DisconnectAsync();
            }
            else if (SelectedDevice != null)
            {
                await ConnectAsync(SelectedDevice);
            }
        }
    }
}
