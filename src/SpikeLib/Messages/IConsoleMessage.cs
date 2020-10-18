using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpikeLib.Messages
{
    public interface IConsoleMessage : IMessage
    {
        bool IsError { get; }
    }
}
