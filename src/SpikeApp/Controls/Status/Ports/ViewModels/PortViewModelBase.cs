using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpikeApp.Utilities;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.Ports.ViewModels
{
    public abstract class PortViewModelBase : ViewModelBase, IPortViewModel
    {
        private string portName = "Port None";
        public string PortName
        {
            get => portName;
            set => RaiseAndSetIfChanged(ref portName, $"Port {value}");
        }

        public abstract void Update(in PortStatus status);
    }
}
