using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using SpikeApp.Utilities;
using SpikeLib.Messages;

namespace SpikeApp.Controls.ViewModels
{
    public class DeviceStatusViewModelOld : ViewModelBase
    {
        private string battery = "Battery: 8.0V 100%";

        public string Battery
        {
            get => battery;
            set => RaiseAndSetIfChanged(ref battery, value);
        }

        private string gyroAngle = "Gyro: X: 0 Y: 0 Z: 0";

        public string GyroAngle
        {
            get => gyroAngle;
            set => RaiseAndSetIfChanged(ref gyroAngle, value);
        }

        private string gyroRate = "Gyro Rate: X: 0, Y: 0, Z: 0";

        public string GyroRate
        {
            get => gyroRate;
            set => RaiseAndSetIfChanged(ref gyroRate, value);
        }

        private string accelerometer = "Accel: X: 0, Y: 0, Z: 0";

        public string Accelerometer
        {
            get => accelerometer;
            set => RaiseAndSetIfChanged(ref accelerometer, value);
        }

        private string gesture = "Gesture: None";

        public string Gesture
        {
            get => gesture;
            set => RaiseAndSetIfChanged(ref gesture, value);
        }

        private PortViewerViewModel? PortA;
        private PortViewerViewModel? PortB;
        private PortViewerViewModel? PortC;
        private PortViewerViewModel? PortD;
        private PortViewerViewModel? PortE;
        private PortViewerViewModel? PortF;


        private void HandlePortStatusMessage(PortStatusMessage message)
        {
            var dirSet = message.GyroAngles;
            GyroAngle = $"Gyro: X: {dirSet.X} Y: {dirSet.Y} Z: {dirSet.Z}";

            dirSet = message.GyroRates;
            GyroRate = $"Gyro Rate: X: {dirSet.X} Y: {dirSet.Y} Z: {dirSet.Z}";

            dirSet = message.Acceleration;
            Accelerometer = $"Accel: X: {dirSet.X} Y: {dirSet.Y} Z: {dirSet.Z}";

            PortA?.UpdateFromStatus(message[PortValue.PortA]);
            PortB?.UpdateFromStatus(message[PortValue.PortB]);
            PortC?.UpdateFromStatus(message[PortValue.PortC]);
            PortD?.UpdateFromStatus(message[PortValue.PortD]);
            PortE?.UpdateFromStatus(message[PortValue.PortE]);
            PortF?.UpdateFromStatus(message[PortValue.PortF]);
        }

        public void SetPortViewerViewModel(PortViewerViewModel viewModel)
        {
            switch (viewModel.Port)
            {
                case "A":
                    PortA = viewModel;
                    break;
                case "B":
                    PortB = viewModel;
                    break;
                case "C":
                    PortC = viewModel;
                    break;
                case "D":
                    PortD = viewModel;
                    break;
                case "E":
                    PortE = viewModel;
                    break;
                case "F":
                    PortF = viewModel;
                    break;
            }
        }

        private void HandleUpdate(IStatusMessage message)
        {
            switch (message)
            {
                case PortStatusMessage portStatus:
                    HandlePortStatusMessage(portStatus);
                    break;
                case BatteryMessage batteryMsg:
                    Battery = $"Battery: {batteryMsg.Voltage}v {batteryMsg.Percentage}%";
                    break;
                case GestureMessage gestureMsg:
                    Gesture = $"Gesture: {gestureMsg.Gesture}";
                    break;
                case ButtonMessage buttonMsg:
                    // TODO
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
