using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.Status.Ports.ViewModels;

namespace SpikeApp.Controls.Status.Ports.Views
{
    public class EmptyPort : UserControl, IPortView
    {
        public EmptyPort()
        {
            throw new NotImplementedException();
        }

        public EmptyPort(string port)
        {
            DataContext = ViewModel;
            PortName = port;
            this.InitializeComponent();
        }

        public string PortName {
            get => ViewModel.PortName;
            set => ViewModel.PortName = value;
        }

        public EmptyPortViewModel ViewModel { get; } = new();

        IPortViewModel IPortView.ViewModel => ViewModel;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
