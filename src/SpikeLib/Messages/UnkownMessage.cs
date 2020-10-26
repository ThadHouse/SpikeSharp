using System;
using System.Text.Json;

namespace SpikeLib.Messages
{
    public class UnknownMessage : IMessage
    {
        public string RawText { get; }

        public UnknownMessage(string msg)
        {
            if (msg == null)
            {
                msg = "Null Message?";
            }

            RawText = msg;
        }
    }
}
