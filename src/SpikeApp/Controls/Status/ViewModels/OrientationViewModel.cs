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
            Orientation = message.Gesture;
        }
    }
}
