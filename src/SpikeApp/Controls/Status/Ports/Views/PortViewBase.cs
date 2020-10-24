using System;
using Avalonia.Controls;
using SpikeApp.Controls.Status.Ports.ViewModels;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.Ports.Views
{
    public class PortViewBase<T> : UserControl, IPortView where T : IPortViewModel, new()
    {
        public PortViewBase() => throw new NotImplementedException();

        public PortViewBase(string port, PortType portType)
        {
            PortType = portType;
            DataContext = ViewModel;
            PortName = port;
        }

        public string PortName
        {
            get => ViewModel.PortName;
            set => ViewModel.PortName = value;
        }

        public T ViewModel { get; } = new();

        public PortType PortType { get; }

        IPortViewModel IPortView.ViewModel => ViewModel;

        public void Update(in PortStatus status)
        {
            ViewModel.Update(status);
        }
    }
}
