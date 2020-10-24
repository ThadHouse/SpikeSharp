using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.Ports.ViewModels
{
    public class ColorSensorViewModel : PortViewModelBase
    {
        private ColorValue color = ColorValue.None;
        public ColorValue Color
        {
            get => color;
            set => RaiseAndSetIfChanged(ref color, value);
        }

        private int reflectivity;
        public int Reflectivity
        {
            get => reflectivity;
            set => RaiseAndSetIfChanged(ref reflectivity, value);
        }

        private int red;
        public int Red
        {
            get => red;
            set => RaiseAndSetIfChanged(ref red, value);
        }

        private int green;
        public int Green
        {
            get => green;
            set => RaiseAndSetIfChanged(ref green, value);
        }

        private int blue;
        public int Blue
        {
            get => blue;
            set => RaiseAndSetIfChanged(ref blue, value);
        }

        public override void Update(in PortStatus status)
        {
            Color = status.GetColor();
            Reflectivity = status.GetColorReflectionPercentage();
            Red = status.GetColorRed();
            Green = status.GetColorGreen();
            Blue = status.GetColorBlue();
        }
    }
}
