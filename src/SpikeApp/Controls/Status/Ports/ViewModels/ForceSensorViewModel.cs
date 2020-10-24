using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.Ports.ViewModels
{
    public class ForceSensorViewModel : PortViewModelBase
    {
        private int force;
        public int Force
        {
            get => force;
            set => RaiseAndSetIfChanged(ref force, value);
        }

        private int pressed;
        public int Pressed
        {
            get => pressed;
            set => RaiseAndSetIfChanged(ref pressed, value);
        }

        private int raw;
        public int Raw
        {
            get => raw;
            set => RaiseAndSetIfChanged(ref raw, value);
        }

        public override void Update(in PortStatus status)
        {
            Force = status.GetForceNewtons();
            Pressed = status.GetForcePressed();
            Raw = status.GetForceRaw();
        }
    }
}
