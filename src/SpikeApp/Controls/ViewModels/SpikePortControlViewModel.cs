using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGen.Runtime.Win32;
using SpikeApp.Utilities;
using SpikeLib;

namespace SpikeApp.Controls.ViewModels
{
    public class SpikePortControlViewModel : ViewModelBase
    {
        public ObservableCollection<string> Devices { get; } = new ObservableCollection<string>();

        public SpikePortControlViewModel()
        {
            RefreshDevices();
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
    }
}
