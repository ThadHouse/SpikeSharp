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

        public void RefreshDevices()
        {
            var allowedDevices = SerialPort.GetPortNames();
            Devices.Clear();
            if (allowedDevices.Length == 0)
            {
                Devices.Add("None");
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
            if (connectText == "Connect")
            {
                if (SelectedDevice == "None") return;
                await ViewModelStorage.AddHubAsync(SelectedDevice);
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
