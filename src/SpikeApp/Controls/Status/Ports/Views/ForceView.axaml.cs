﻿using System;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.Status.Ports.ViewModels;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.Ports.Views
{
    public class ForceView : PortViewBase<ForceSensorViewModel>
    {
        public ForceView() => throw new NotImplementedException();

        public ForceView(string port) : base(port, PortType.ForceSensor)
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
