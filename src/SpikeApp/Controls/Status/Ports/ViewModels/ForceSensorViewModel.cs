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

        public override void Update(in PortStatus status)
        {
            Force = status.GetForce();
        }
    }
}
