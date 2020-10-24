using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.Status.Ports.ViewModels;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.Ports.Views
{
    public class EmptyPortView : PortViewBase<EmptyPortViewModel>
    {
        public EmptyPortView() => throw new NotImplementedException();

        public EmptyPortView(string port) : base(port, PortType.None)
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
