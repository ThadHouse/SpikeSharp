using System;
using System.Text.Json;

namespace SpikeLib.Messages
{
    public class UnknownMessage : IMessage
    {
        public string RawText { get; }

        public UnknownMessage(JsonDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            RawText = document.RootElement.GetRawText();
        }
    }
}
