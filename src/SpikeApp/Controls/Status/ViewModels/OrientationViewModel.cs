using SpikeApp.Utilities;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.ViewModels
{
    public class OrientationViewModel : ViewModelBase
    {
        private string orientation = "None";
        public string Orientation
        {
            get => orientation;
            set => RaiseAndSetIfChanged(ref orientation, value);
        }

        public void Update(GestureMessage message)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            Orientation = message.Gesture;
#pragma warning restore CA1062 // Validate arguments of public methods
        }
    }
}
