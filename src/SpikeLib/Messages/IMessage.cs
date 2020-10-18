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
                    case 3:
                        // Button Press
                        break;
                    case 4:
                        // Gesture
                        break;
                    case 12:
                        // Program start [ "prog name", true for start, false for stop
                        return new ProgramStateChangedMessage(document);
                    case 7:
                    case 8:
                        // Initialization?
                        break;
                    case 11:
                        // Program status
                        break;

                }
            }
            else if (methodProperty.ValueKind == JsonValueKind.String)
            {
                var methodStr = methodProperty.GetString();
                if (methodStr == "user_program_error")
                {
                    return new UserProgramErrorMessage(document);
                }
            }            
            return new UnknownMessage(document.RootElement.GetRawText());
        }
    }
}
