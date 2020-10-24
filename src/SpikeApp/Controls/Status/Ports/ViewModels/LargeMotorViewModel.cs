using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.Ports.ViewModels
{
    public class LargeMotorViewModel : PortViewModelBase
    {
        private int power;
        public int Power
        {
            get => power;
            set => RaiseAndSetIfChanged(ref power, value);
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
            Power = status.GetMotorPower();
            Angle = status.GetMotorAngle();
            AbsoulteAngle = status.GetMotorAbsoluteAngle();
            Rate = status.GetMotorRate();
        }
    }
}
