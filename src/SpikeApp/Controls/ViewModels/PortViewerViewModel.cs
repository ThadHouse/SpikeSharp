using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpikeApp.Utilities;
using SpikeLib.Messages;

namespace SpikeApp.Controls.ViewModels
{
    public class PortViewerViewModel : ViewModelBase
    {
        private string port = "Port";
        public string Port
        {
            get => port;
            set
            {
                port = value;
                PortText = $"Port {value}";
                //ViewModelStorage.SetPortViewerViewModel(this);
            }
        }

        private string portText = "Port ";

        public string PortText
        {
            get => portText;
            set => RaiseAndSetIfChanged(ref portText, value);
        }

        private string typeText = "None";

        public string TypeText
        {
            get => typeText;
            set => RaiseAndSetIfChanged(ref typeText, value);
        }

        private string statusText = "";

        public string StatusText
        {
            get => statusText;
            set => RaiseAndSetIfChanged(ref statusText, value);
        }

        public void UpdateFromStatus(PortStatus status)
        {
            switch (status.Type)
            {
                case PortType.None:
                    TypeText = "None";
                    StatusText = "";
                    break;
                case PortType.MediumMotor:
                    TypeText = "Medium Motor";
                    StatusText = $"Rate: {status.GetMotorRate()} Angle: {status.GetMotorAngle()} Abs Angle: {status.GetMotorAbsoluteAngle()}";
                    break;
                case PortType.LargeMotor:
                    TypeText = "Large Motor";
                    StatusText = $"Rate: {status.GetMotorRate()} Angle: {status.GetMotorAngle()} Abs Angle: {status.GetMotorAbsoluteAngle()}";
                    break;
                case PortType.UltrasonicSensor:
                    TypeText = "Ultrasonic";
                    break;
                case PortType.ColorSensor:
                    TypeText = "Color";
                    break;
                case PortType.ForceSensor:
                    TypeText = "Force";
                    break;
                case PortType.Unknown:
                default:
                    TypeText = "Unknown";
                    StatusText = "";
                    break;
            }
        }
    }
}
