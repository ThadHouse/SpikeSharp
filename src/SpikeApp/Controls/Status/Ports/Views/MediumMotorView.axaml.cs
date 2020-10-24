using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.Status.Ports.ViewModels;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.Ports.Views
{
    public class MediumMotorView : PortViewBase<MediumMotorViewModel>
    {
        public MediumMotorView() => throw new NotImplementedException();

        public MediumMotorView(string port) : base(port, PortType.MediumMotor)
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
