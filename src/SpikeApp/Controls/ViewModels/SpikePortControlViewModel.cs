using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpikeApp.Utilities;

namespace SpikeApp.Controls.ViewModels
{
    public class SpikePortControlViewModel : ViewModelBase
    {
        public ObservableCollection<string> Devices { get; } = new ObservableCollection<string>();

        public SpikePortControlViewModel()
        {
            Devices.Add("None");
        }

        public string SelectedDevice { get; set; } = "None";

        private string connectText = "Connect";

        public string ConnectText
        {
            get => connectText;
            set => RaiseAndSetIfChanged(ref connectText, value);
        }

        public void RefreshDevices()
        {

        }

        public void Connect()
        {

        }
    }
}
