using System;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.Status.Ports.ViewModels;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.Ports.Views
{
    public class LargeMotorView : PortViewBase<LargeMotorViewModel>
    {
        public LargeMotorView() => throw new NotImplementedException();

        public LargeMotorView(string port) : base(port, PortType.MediumMotor)
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
