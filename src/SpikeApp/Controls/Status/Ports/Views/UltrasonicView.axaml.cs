using System;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.Status.Ports.ViewModels;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.Ports.Views
{
    public class UltrasonicView : PortViewBase<UltrasonicViewModel>
    {
        public UltrasonicView() => throw new NotImplementedException();

        public UltrasonicView(string port) : base(port, PortType.UltrasonicSensor)
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
