using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using SharpGen.Runtime.Win32;
using SpikeApp.Utilities;
using SpikeLib;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Devices.Usb;

namespace SpikeApp.Controls.ViewModels
{
    public class SpikePortControlViewModel : ViewModelBase
    {
        private class HubInfo
        {
            public DeviceInformation DeviceInfo { get; }
            public HubInfo(DeviceInformation info)
            {
                DeviceInfo = info;
            }
            public string Id => DeviceInfo.Id;
        }

        public ObservableCollection<string> Devices { get; } = new ObservableCollection<string>();

        private readonly ConcurrentDictionary<string, HubInfo> connectedDeviceInfo = new();
        private readonly DeviceWatcher deviceWatcherSerial;
        private readonly DeviceWatcher deviceWatcherBluetooth;
        private readonly SynchronizationContext context;
        private readonly SendOrPostCallback sendOrPostCb;

        private async void TriggerRefresh(object? o)
        {
            if (o is DeviceInformation devInfo)
            {
                ;
            }
            else if (o is DeviceInformationUpdate devInfoUpdate)
            {
                ;
            }
            ;
        }

        public SpikePortControlViewModel()
        {
            sendOrPostCb = TriggerRefresh;
            context = SynchronizationContext.Current!;
            var deviceClass = BluetoothDevice.GetDeviceSelectorFromClassOfDevice(BluetoothClassOfDevice.FromParts(BluetoothMajorClass.Toy, BluetoothMinorClass.ToyRobot, BluetoothServiceCapabilities.None));
            deviceWatcherBluetooth = DeviceInformation.CreateWatcher(deviceClass, new string[]
            {
            "System.ItemNameDisplay",
            "System.Devices.Aep.IsConnected",
            "System.Devices.Aep.DeviceAddress",
            "System.Devices.Aep.ProtocolId",
            "System.Devices.Aep.SignalStrength"
            });
            deviceWatcherSerial = DeviceInformation.CreateWatcher(SerialDevice.GetDeviceSelectorFromUsbVidPid(0x0694, 0x0010));

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
            var id = args.Id;
            if (connectedDeviceInfo.TryGetValue(id, out var info))
            {
                info.DeviceInfo.Update(args);
            }
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            var id = args.Id;
            if (connectedDeviceInfo.TryRemove(id, out var _))
            {
                context.Post(sendOrPostCb, args);
            }
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
            var id = args.Id;
            connectedDeviceInfo.TryRemove(id, out var _);
            connectedDeviceInfo.TryAdd(id, new HubInfo(args));
            context.Post(sendOrPostCb, args);
        }

        private string selectedDevice = "None";
            
        public string SelectedDevice
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

        public async void RefreshDevices()
        {
            await RefreshDevicesAsync();
        }

        public async void RefreshBluetoothDevices()
        {
            await RefreshBluetoothDevicesAsync();
        }

        public async Task RefreshBluetoothDevicesAsync()
        {
            
            //var devices = await DeviceInformation.FindAllAsync(deviceClass, new string[]
            //{
            //"System.ItemNameDisplay",
            //"System.Devices.Aep.IsConnected",
            //"System.Devices.Aep.DeviceAddress",
            //"System.Devices.Aep.ProtocolId",
            //"System.Devices.Aep.SignalStrength"
            //});
            
            //devices.Where(x => x.)
        }

        public async Task RefreshDevicesAsync()
        {
            var allowedDevices = await SerialSpikeConnection.EnumerateConnectedHubsAsync();
            Devices.Clear();
            if (allowedDevices.Count == 0)
            {
                Devices.Add("None");
            }
            else if (allowedDevices.Count == 1 && connectText == "Connect")
            {
                Devices.Add(allowedDevices[0]);
                SelectedDevice = allowedDevices[0];
                await ConnectAsync();
            }
            else
            {
                foreach (var d in allowedDevices)
                {
                    Devices.Add(d);
                }
                SelectedDevice = allowedDevices[0];
            }
        }

        public async void Connect()
        {
            await ConnectAsync();
        }

        public async Task ConnectAsync()
        {
            if (connectText == "Connect")
            {
                if (SelectedDevice == "None") return;
                var device = await SerialSpikeConnection.OpenConnectionAsync(SelectedDevice);
                if (device == null) return;
                await ViewModelStorage.AddHubAsync(device);
                ConnectText = "Disconnect";
            }
            else
            {
                await ViewModelStorage.CloseHubAsync();
                ConnectText = "Connect";
            }
        }

        public void ScanBluetooth()
        {

        }
    }
}
