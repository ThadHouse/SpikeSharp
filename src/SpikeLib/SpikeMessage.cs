using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpikeLib
{
    public enum SpikeMessageType
    {
        PortState,
        BrickState,
        ProgramChanged,
        UserProgramError
    }

    public readonly struct SpikeMessage
    {
        public SpikeMessageType MessageType { get; }


    }
}
