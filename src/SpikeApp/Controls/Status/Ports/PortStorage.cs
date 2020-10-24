using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SpikeApp.Controls.Status.Ports.Views;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.Ports
{

    public class PortStorage
    {
        private readonly ImmutableArray<ImmutableDictionary<PortType, IPortView>> AllowedPortList;
        private readonly int startIndex;
        private readonly Avalonia.Controls.Controls portControls;

        private readonly IPortView[] currentPorts = new IPortView[6];

        public PortStorage(Avalonia.Controls.Controls controls)
        {
            if (controls == null) throw new ArgumentNullException(nameof(controls));

            startIndex = controls.Count;
            portControls = controls;

            string[] portMap = new string[] { "A", "B", "C", "D", "E", "F" };
            List<ImmutableDictionary<PortType, IPortView>> ports = new();
            for (int i = 0; i < 6; i++)
            {
                var portValue = portMap[i];
                Dictionary<PortType, IPortView> port = new();
                port[PortType.None] = new EmptyPortView(portValue);
                port[PortType.ColorSensor] = new ColorSensorView(portValue);
                port[PortType.UltrasonicSensor] = new UltrasonicView(portValue);
                port[PortType.MediumMotor] = new MediumMotorView(portValue);
                port[PortType.LargeMotor] = new LargeMotorView(portValue);
                port[PortType.ForceSensor] = new ForceView(portValue);
                port[PortType.Unknown] = new UnknownPortView(portValue);
                ports.Add(port.ToImmutableDictionary());
            }
            AllowedPortList = ports.ToImmutableArray();

            for (int i = 0; i < 6; i++)
            {
                var port = AllowedPortList[i][PortType.None];
                portControls.Add(port);
                currentPorts[i] = port;
            }
        }

        public void Update(PortStatusMessage message)
        {
            for (int i = 0; i < 6; i++)
            {
#pragma warning disable CA1062 // Validate arguments of public methods
                ref readonly PortStatus status = ref message[(PortValue)i];
#pragma warning restore CA1062 // Validate arguments of public methods
                var currentPort = currentPorts[i];
                if (status.Type != currentPort.PortType)
                {
                    var canSetDict = AllowedPortList[i];
                    if (!canSetDict.TryGetValue(status.Type, out currentPort))
                    {
                        currentPort = canSetDict[PortType.None];
                    }
                    currentPorts[i] = currentPort;
                    portControls[startIndex + i] = currentPort;
                }
                currentPort.Update(status);
            }
        }

        
    }
}
