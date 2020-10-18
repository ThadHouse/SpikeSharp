using System;
using System.Text.Json;

namespace SpikeLib.Messages
{
    public class BatteryMessage : IStatusMessage
    {
        public int Percentage { get; }
        public float Voltage { get; }

        public string RawText { get; }

        public BatteryMessage(JsonDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            RawText = document.RootElement.GetRawText();
            var properties = document.RootElement.GetProperty(stackalloc byte[] { (byte)'p' });

            Voltage = properties[0].GetSingle();
            Percentage = properties[1].GetInt32();
        }
    }
}
