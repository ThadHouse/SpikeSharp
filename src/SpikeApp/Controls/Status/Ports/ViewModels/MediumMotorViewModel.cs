using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.Ports.ViewModels
{
    public class MediumMotorViewModel : PortViewModelBase
    {
        private int currentPercentage;
        public int CurrentPercentage
        {
            get => currentPercentage;
            set => RaiseAndSetIfChanged(ref currentPercentage, value);
        }

        private int angle;
        public int Angle
        {
            get => angle;
            set => RaiseAndSetIfChanged(ref angle, value);
        }

        private int rate;
        public int Rate
        {
            get => rate;
            set => RaiseAndSetIfChanged(ref rate, value);
        }

        private int absoulteAngle;
        public int AbsoulteAngle
        {
            get => absoulteAngle;
            set => RaiseAndSetIfChanged(ref absoulteAngle, value);
        }

        public override void Update(in PortStatus status)
        {
            CurrentPercentage = status.GetMotorFaults();
            Angle = status.GetMotorAngle();
            AbsoulteAngle = status.GetMotorAbsoluteAngle();
            Rate = status.GetMotorRate();
        }
    }
}
