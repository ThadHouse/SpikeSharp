using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using SpikeApp.Controls.Status.Ports.ViewModels;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.Ports.Views
{
    public interface IPortView : IControl
    {
        string PortName { get; set; }
        IPortViewModel ViewModel { get; }
        PortType PortType { get; }
        void Update(in PortStatus status);
    }
}
