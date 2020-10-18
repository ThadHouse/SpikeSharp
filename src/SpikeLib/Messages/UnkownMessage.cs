using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpikeLib.Messages
{
    public class UnknownMessage : IMessage
    {
        public string RawText { get; }

        public UnknownMessage(JsonDocument document)
        {
            RawText = document.RootElement.GetRawText();
        }
    }
}
