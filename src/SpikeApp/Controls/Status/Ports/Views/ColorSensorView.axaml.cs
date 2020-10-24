using System;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.Status.Ports.ViewModels;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.Ports.Views
{
    public class ColorSensorView : PortViewBase<ColorSensorViewModel>
    {
        public ColorSensorView() => throw new NotImplementedException();

        public ColorSensorView(string port) : base(port, PortType.ColorSensor)
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
