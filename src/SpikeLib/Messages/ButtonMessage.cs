using System;
using System.Text.Json;

namespace SpikeLib.Messages
{
    public class ButtonMessage : IStatusMessage
    {
        // left, center, right, connect
        // ["button", 0 for initial press, positive on release, press length]
        public string RawText { get; }

        public ButtonMessage(JsonDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            RawText = document.RootElement.GetRawText();
        }
    }
}
