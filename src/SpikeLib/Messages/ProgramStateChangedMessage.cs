using System;
using System.Text;
using System.Text.Json;

namespace SpikeLib.Messages
{
    public class ProgramStateChangedMessage : IMessage
    {
        public bool Started { get; }
        public string ProgramName { get; }

        public string RawText { get; }

        public ProgramStateChangedMessage(JsonDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            RawText = document.RootElement.GetRawText();
            var properties = document.RootElement.GetProperty(stackalloc byte[] { (byte)'p' });

            Started = properties[1].GetBoolean();
            var strVal = properties[0].GetString()!;
            var base64 = Convert.FromBase64String(strVal);
            ProgramName = Encoding.UTF8.GetString(base64);
            ;
        }

        public override string ToString()
        {
            return $"{ProgramName} was {(Started ? "started" : "stopped")}";
        }
    }
}
