using System;
using System.Text.Json;

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

    public enum PortValue
    {
        PortA,
        PortB,
        PortC,
        PortD,
        PortE,
        PortF
    }

    public enum ColorValue
    {
        None = -1,
        Black,
        Violet,
        Blue,
        Cyan,
        Green,
        Yellow,
        Red,
        White
    }

    public readonly struct PortStatus : IEquatable<PortStatus>
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
            return value0;
        }

        public int GetMotorAngle()
        {
            return value1;
        }

        public int GetMotorAbsoluteAngle()
        {
            return value2;
        }

        public int GetMotorFaults()
        {
            return value3;
        }

        public int GetColorReflectionPercentage()
        {
            return value0;
        }

        public int GetForce()
        {
            return value0;
        }

        public ColorValue GetColor()
        {
            return (ColorValue)value1;
        }

        public int GetColorRed() => value2;
        public int GetColorGreen() => value3;
        public int GetColorBlue() => value4;

        public int GetDistanceCm() => value0;

        public override bool Equals(object? obj)
        {
            return obj is PortStatus status && Equals(status);
        }

        public bool Equals(PortStatus other)
        {
            return Type == other.Type &&
                   value0 == other.value0 &&
                   value1 == other.value1 &&
                   value2 == other.value2 &&
                   value3 == other.value3 &&
                   value4 == other.value4;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, value0, value1, value2, value3, value4);
        }

        public static bool operator ==(PortStatus left, PortStatus right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PortStatus left, PortStatus right)
        {
            return !(left == right);
        }
    }

    public readonly struct DirectionSet : IEquatable<DirectionSet>
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

        public override bool Equals(object? obj)
        {
            return obj is DirectionSet set && Equals(set);
        }

        public bool Equals(DirectionSet other)
        {
            return X == other.X &&
                   Y == other.Y &&
                   Z == other.Z;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public static bool operator ==(DirectionSet left, DirectionSet right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DirectionSet left, DirectionSet right)
        {
            return !(left == right);
        }
    }
    
    public class PortStatusMessage : IStatusMessage
    {
        private readonly PortStatus portA;
        private readonly PortStatus portB;
        private readonly PortStatus portC;
        private readonly PortStatus portD;
        private readonly PortStatus portE;
        private readonly PortStatus portF;

        private readonly DirectionSet acceleration;
        private readonly DirectionSet gyroRates;
        private readonly DirectionSet gyroAngles;

        public ref readonly DirectionSet Acceleration => ref acceleration;
        public ref readonly DirectionSet GyroRates => ref gyroRates;
        public ref readonly DirectionSet GyroAngles => ref gyroAngles;

        public string RawText { get; }

        public ref readonly PortStatus this[PortValue value]
        {
            get
            {
                switch (value)
                {
                    case PortValue.PortA:
                        return ref portA;
                    case PortValue.PortB:
                        return ref portB;
                    case PortValue.PortC:
                        return ref portC;
                    case PortValue.PortD:
                        return ref portD;
                    case PortValue.PortE:
                        return ref portE;
                    case PortValue.PortF:
                        return ref portF;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }

        public PortStatusMessage(JsonDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            RawText = document.RootElement.GetRawText();
            // Medium Motor : 75  [Rate, Angle, Absoulte Postition, fault?]
            // Light Sensor: 61 [reflectivity, color (or null), r, g, b]
            // Ultrasonic : 62 [cm (or null)]

            var properties = document.RootElement.GetProperty(stackalloc byte[] { (byte)'p' });


            SetPort(out PortStatus port, properties[0]);
            portA = port;

            SetPort(out port, properties[1]);
            portB = port;

            SetPort(out port, properties[2]);
            portC = port;

            SetPort(out port, properties[3]);
            portD = port;

            SetPort(out port, properties[4]);
            portE = port;

            SetPort(out port, properties[5]);
            portF = port;


            acceleration = new DirectionSet(properties[6]);
            gyroRates = new DirectionSet(properties[7]);
            gyroAngles = new DirectionSet(properties[8]);


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
