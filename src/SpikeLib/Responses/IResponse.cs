using SpikeLib.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpikeLib.Responses
{
    public interface IResponse : IMessage
    {
        string Id { get; }
    }
}
