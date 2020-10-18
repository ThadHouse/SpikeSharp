using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpikeLib.Messages
{
    public enum PortType
    {
        None,
        MediumMotor,
        LargeMotor,
        UltrasonicSensor,
        ColorSensor,
        ForceSensor,
        Unknown
    }

    public enum ColorValue
    {
        None = -1,
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K
    }

    public readonly struct PortStatus
    {
        public PortType Type { get; }
        private readonly int value0, value1, value2, value3, value4;

        internal PortStatus(PortType type, int value0 = 0, int value1 = 0, int value2 = 0, int value3 = 0, int value4 = 0)
        {
            Type = type;
            this.value0 = value0;
            this.value1 = value1;
            this.value2 = value2;
            this.value3 = value3;
            this.value4 = value4;
        }

        public int GetMotorRate()
        {
            if (Type != PortType.MediumMotor && Type != PortType.LargeMotor)
            {
                throw new InvalidOperationException("Cannot read rate of non motor");
            }
            return value0;
        }

        public int GetMotorAngle()
        {
            if (Type != PortType.MediumMotor && Type != PortType.LargeMotor)
            {
                throw new InvalidOperationException("Cannot read angle of non motor");
            }
            return value1;
        }

        public int GetMotorAbsoluteAngle()
        {
            if (Type != PortType.MediumMotor && Type != PortType.LargeMotor)
            {
                throw new InvalidOperationException("Cannot read absolute angle of non motor");
            }
            return value2;
        }

        public int GetColorReflectionPercentage()
        {
            if (Type != PortType.ColorSensor)
            {
                throw new InvalidOperationException("Cannot read color sensor value");
            }
            return value0;
        }

        public ColorValue GetColor()
        {
            if (Type != PortType.ColorSensor)
            {
                throw new InvalidOperationException();
            }
            return (ColorValue)value1;
        }
    }

    public readonly struct DirectionSet
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }
        internal DirectionSet(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        internal DirectionSet(JsonElement elements)
        {
            X = elements[0].GetInt32();
            Y = elements[1].GetInt32();
            Z = elements[2].GetInt32();
        }
    }
    
    public class PortStatusMessage : IStatusMessage
    {
        public PortStatus PortA { get; }
        public PortStatus PortB { get; }
        public PortStatus PortC { get; }
        public PortStatus PortD { get; }
        public PortStatus PortE { get; }
        public PortStatus PortF { get; }

        public DirectionSet Acceleration { get; }
        public DirectionSet GyroRates { get; }
        public DirectionSet GyroAngles { get; }

        public PortStatusMessage(JsonDocument document)
        {
            // Medium Motor : 75  [Rate, Angle, Absoulte Postition, fault?]
            // Light Sensor: 61 [reflectivity, color (or null), r, g, b]
            // Ultrasonic : 62 [cm (or null)]

            var properties = document.RootElement.GetProperty(stackalloc byte[] { (byte)'p' });


            SetPort(out PortStatus port, properties[0]);
            PortA = port;

            SetPort(out port, properties[1]);
            PortB = port;

            SetPort(out port, properties[2]);
            PortC = port;

            SetPort(out port, properties[3]);
            PortD = port;

            SetPort(out port, properties[4]);
            PortE = port;

            SetPort(out port, properties[5]);
            PortF = port;


            Acceleration = new DirectionSet(properties[6]);
            GyroRates = new DirectionSet(properties[7]);
            GyroAngles = new DirectionSet(properties[8]);


            // Order is 6 ports, Acceleration, Gryo Rates, Gyro Angles, Unknown string, 0
        }

        private static void SetPort(out PortStatus port, JsonElement portProperty)
        {
            int portType = portProperty[0].GetInt32();
            var values = portProperty[1];
            switch (portType)
            {
                case 0:
                    port = new PortStatus(PortType.None);
                    break;
                case 75:
                    port = new PortStatus(PortType.MediumMotor, values[0].GetInt32(), values[1].GetInt32(), values[2].GetInt32(), values[3].GetInt32());
                    break;
                case 61:
                    var val1 = values[1];
                    if (val1.ValueKind == JsonValueKind.Null)
                    {
                        port = new PortStatus(PortType.ColorSensor, values[0].GetInt32(), -1, values[2].GetInt32(), values[3].GetInt32(), values[4].GetInt32());
                    }
                    else
                    {
                        port = new PortStatus(PortType.ColorSensor, values[0].GetInt32(), values[1].GetInt32(), values[2].GetInt32(), values[3].GetInt32(), values[4].GetInt32());
                    }
                    break;
                case 62:
                    var val0 = values[0];
                    if (val0.ValueKind == JsonValueKind.Null)
                    {
                        port = new PortStatus(PortType.UltrasonicSensor, -1);
                    }
                    else
                    {
                        port = new PortStatus(PortType.UltrasonicSensor, val0.GetInt32());
                    }
                    break;
                default:
                    port = new PortStatus(PortType.Unknown);
                    break;
            }

        }
    }
}
