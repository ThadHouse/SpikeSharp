using SpikeApp.Utilities;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.ViewModels
{
    public class ThreeAxisViewModel : ViewModelBase
    {
        private string typeName = "None";
        public string TypeName
        {
            get => typeName;
            set => RaiseAndSetIfChanged(ref typeName, value);
        }

        private int x;
        private int y;
        private int z;

        public int X
        {
            get => x;
            set => RaiseAndSetIfChanged(ref x, value);
        }

        public int Y
        {
            get => y;
            set => RaiseAndSetIfChanged(ref y, value);
        }

        public int Z
        {
            get => z;
            set => RaiseAndSetIfChanged(ref z, value);
        }

        public void Update(in DirectionSet directions)
        {
            X = directions.X;
            Y = directions.Y;
            Z = directions.Z;
        }
    }
}
