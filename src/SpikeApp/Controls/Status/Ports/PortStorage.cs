using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SpikeApp.Controls.Status.Ports.Views;
using SpikeLib.Messages;
using AVGrid = Avalonia.Controls.Grid;
using AVControl = Avalonia.Controls.Control;
using AVControls = Avalonia.Controls.Controls;

namespace SpikeApp.Controls.Status.Ports
{

    public class PortStorage
    {
        private readonly ImmutableArray<ImmutableDictionary<PortType, IPortView>> AllowedPortList;
        private readonly AVGrid deviceGrid;
        private readonly AVControls controls;

        private readonly IPortView[] currentPorts = new IPortView[6];

        private readonly (int row, int column)[] gridArray = new (int row, int column)[]
        {
            (0, 0),
            (0, 1),
            (1, 0),
            (1, 1),
            (2, 0),
            (2, 1)
        };

        public PortStorage(AVGrid deviceGrid)
        {
            if (deviceGrid == null) throw new ArgumentNullException(nameof(deviceGrid));


            this.deviceGrid = deviceGrid;
            controls = deviceGrid.Children;

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
                foreach (var addedPort in port)
                {
                    (int row, int column) = gridArray[i];
                    addedPort.Value.SetValue(AVGrid.RowProperty, row);
                    addedPort.Value.SetValue(AVGrid.ColumnProperty, column);
                }
                ports.Add(port.ToImmutableDictionary());
            }
            AllowedPortList = ports.ToImmutableArray();

            for (int i = 0; i < 6; i++)
            {
                var port = AllowedPortList[i][PortType.None];
                
                deviceGrid.Children.Add(port);
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
                    controls[i] = currentPort;
                    
                }
                currentPort.Update(status);
            }
        }

        
    }
}
