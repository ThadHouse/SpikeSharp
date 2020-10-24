using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using SpikeApp.Controls.Status.Ports.Views;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.Ports
{
    public class PortStorage
    {
        public ImmutableArray<ImmutableDictionary<PortType, IPortView>> Ports { get; }
        public int StartIndex { get; }
        public Avalonia.Controls.Controls PortControls { get; }

        public PortStorage(Avalonia.Controls.Controls controls)
        {
            StartIndex = controls.Count;
            PortControls = controls;

            string[] portMap = new string[] { "A", "B", "C", "D", "E", "F" };
            List<ImmutableDictionary<PortType, IPortView>> ports = new();
            for (int i = 0; i < 6; i++)
            {
                var portValue = portMap[i];
                Dictionary<PortType, IPortView> port = new();
                port[PortType.None] = new EmptyPort(portValue);
                ports.Add(port.ToImmutableDictionary());
            }
            Ports = ports.ToImmutableArray();

            for (int i = 0; i < 6; i++)
            {
                PortControls.Add(Ports[i][PortType.None]);
            }
        }
    }
}
