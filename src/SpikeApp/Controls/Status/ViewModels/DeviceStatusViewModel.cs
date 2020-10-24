using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using SpikeApp.Controls.Status.Ports;
using SpikeApp.Utilities;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.ViewModels
{
    public class DeviceStatusViewModel : ViewModelBase
    {
        private readonly BatteryViewModel battery;
        private readonly ThreeAxisViewModel gyroAngle;
        private readonly ThreeAxisViewModel gyroRate;
        private readonly ThreeAxisViewModel accelerometer;
        private readonly OrientationViewModel orientation;
        private readonly PortStorage portStorage;

        public DeviceStatusViewModel(BatteryViewModel battery, ThreeAxisViewModel gyroAngle, ThreeAxisViewModel gyroRate, ThreeAxisViewModel accelerometer, OrientationViewModel orientation, PortStorage portStorage)
        {
            this.battery = battery;
            this.gyroAngle = gyroAngle;
            this.gyroRate = gyroRate;
            this.accelerometer = accelerometer;
            this.orientation = orientation;
            this.portStorage = portStorage;
        }

        private void HandleGeneralStatusUpdate(PortStatusMessage message)
        {
            gyroAngle.Update(message.GyroAngles);
            gyroRate.Update(message.GyroRates);
            accelerometer.Update(message.Acceleration);
            portStorage.Update(message);
        }

        private void HandleUpdate(IStatusMessage message)
        {
            switch (message)
            {
                case BatteryMessage bm:
                    battery.Update(bm);
                    break;
                case PortStatusMessage psm:
                    HandleGeneralStatusUpdate(psm);
                    break;
                case GestureMessage gm:
                    orientation.Update(gm);
                    break;
            }
        }

        private Task? channelReaderTask;

        private async Task ChannelReaderFuncAsync(ChannelReader<IStatusMessage> reader)
        {
            while (true)
            {
                try
                {
                    var element = await reader.ReadAsync();
                    HandleUpdate(element);
                }
                catch (ChannelClosedException)
                {
                    break;
                }
            }
        }

        public void AddChannelReader(ChannelReader<IStatusMessage> reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            channelReaderTask = ChannelReaderFuncAsync(reader);
        }

        public async Task RemoveChannelReaderAsync()
        {
            if (channelReaderTask != null)
            {
                await channelReaderTask;
            }
            channelReaderTask = null;
        }
    }
}
