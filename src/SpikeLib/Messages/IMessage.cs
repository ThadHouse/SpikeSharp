using SpikeLib.Responses;
using System;
using System.Text.Json;

namespace SpikeLib.Messages
{
    public interface IMessage
    {
        public static IMessage? ParseMessage(JsonDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (document.RootElement.TryGetProperty(stackalloc byte[] { (byte)'m' }, out var methodProperty))
            {
                if (methodProperty.ValueKind == JsonValueKind.Number)
                {
                    switch (methodProperty.GetInt32())
                    {
                        case 1:
                            // Storage response, replied upon successful file upload
                            break;
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
                    else if (methodStr == "userProgram.print")
                    {
                        return new UserProgramPrintMessage(document);
                    }
                }
                return new UnknownMessage(document);
            }
            else if (document.RootElement.TryGetProperty(stackalloc byte[] { (byte) 'i'}, out var idProperty))
            {
                var idVal = idProperty.GetString()!;
                char idChar = idVal[0];
                switch (idChar)
                {
                    case '0':
                        return new StorageResponse(idVal, document);
                    case '1':
                        return new StartWriteProgramResponse(idVal, document);
                    case '2':
                        return new WritePackageResponse(idVal, document);
                }
                return new UnknownMessage(document);
            }
            else
            {
                return null;
            }
        }

        string RawText { get; }
    }
}
