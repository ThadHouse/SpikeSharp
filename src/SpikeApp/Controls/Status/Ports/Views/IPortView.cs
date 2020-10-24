using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using SpikeApp.Controls.Status.Ports.ViewModels;

namespace SpikeApp.Controls.Status.Ports.Views
{
    public interface IPortView : IControl
    {
        public string PortName { get; set; }
        public IPortViewModel ViewModel { get; }
    }
}
