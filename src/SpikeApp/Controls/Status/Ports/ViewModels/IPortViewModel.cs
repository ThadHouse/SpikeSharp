﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.Ports.ViewModels
{
    public interface IPortViewModel
    {
        string PortName { get; set; }
        void Update(in PortStatus status);
    }
}
