using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpikeLib.Messages
{
    public class ButtonMessage : IStatusMessage
    {
        // left, center, right, connect
        // ["button", 0 for initial press, positive on release, press length]
        public string RawText { get; }

        public ButtonMessage(JsonDocument document)
        {
            RawText = document.RootElement.GetRawText();
        }
    }
}
