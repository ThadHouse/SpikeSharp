using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpikeLib.Messages
{
    public interface IMessage
    {
        public static IMessage? ParseMessage(JsonDocument document)
        {
            var methodProperty = document.RootElement.GetProperty(stackalloc byte[] { (byte)'m' });
            if (methodProperty.ValueKind == JsonValueKind.Number)
            {
                switch (methodProperty.GetInt32())
                {
                    case 0:
                        // Port Status
                        return new PortStatusMessage(document);
                    case 2:
                        // Battery [voltage, percentage]
                        return new BatteryMessage(document);
                    case 4:
                        // Gesture
                        break;
                }
            }
            else if (methodProperty.ValueKind == JsonValueKind.String)
            {

            }            
            return null;
        }
    }
}
