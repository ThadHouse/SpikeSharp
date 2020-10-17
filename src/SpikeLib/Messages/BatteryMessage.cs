using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpikeLib.Messages
{
    public class BatteryMessage : IMessage
    {
        public int Percentage { get; }
        public float Voltage { get; }

        public BatteryMessage(JsonDocument document)
        {
            var properties = document.RootElement.GetProperty(stackalloc byte[] { (byte)'p' });

            Voltage = properties[0].GetSingle();
            Percentage = properties[1].GetInt32();
        }
    }
}
