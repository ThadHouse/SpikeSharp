using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.Status.Ports.ViewModels;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.Ports.Views
{
    public class UnknownPortView : PortViewBase<EmptyPortViewModel>
    {
        public UnknownPortView() => throw new NotImplementedException();

        public UnknownPortView(string port) : base(port, PortType.Unknown)
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
