using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.Ports.ViewModels
{
    public class UltrasonicViewModel : PortViewModelBase
    {
        private int distance = -1;
        public int Distance
        {
            get => distance;
            set => RaiseAndSetIfChanged(ref distance, value);
        }

        public override void Update(in PortStatus status)
        {
            Distance = status.GetDistanceCm();
        }
    }
}
