﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpikeLib.Messages
{
    public enum Gesture
    {
        Tapped,
        Down,
        Front,
        Up,
        Back,
        Rightside,
        Leftside,
        Freefall,
        DoubleTapped,
        Shake
    }
    public class GestureMessage
    {
    }
}
