using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpikeLib.Messages
{
    public class UnknownMessage : IMessage
    {
        private readonly string rawMessage;

        public UnknownMessage(string rawMessage)
        {
            this.rawMessage = rawMessage;
        }

        public override string ToString()
        {
            return rawMessage;
        }
    }
}
