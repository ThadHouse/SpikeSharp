using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.Status.Ports;
using SpikeApp.Controls.Status.ViewModels;
using SpikeApp.Controls.ViewModels;
using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.Views
{

    public class DeviceStatusView : UserControl
    {
        public DeviceStatusView()
        {
            this.InitializeComponent();
            var battery = this.FindControl<BatteryView>("Battery");
            var gyroAngle = this.FindControl<ThreeAxisView>("GyroAngle");
            var gyroRate = this.FindControl<ThreeAxisView>("GyroRate");
            var accel = this.FindControl<ThreeAxisView>("Accel");
            var orientation = this.FindControl<OrientationView>("Orientation");

            var scrollView = (ScrollViewer)this.Content;
            var stackPanel = (StackPanel)scrollView.Content;

            PortStorage storage = new(stackPanel.Children);            

            DeviceStatusViewModel vm = new(battery.ViewModel, gyroAngle.ViewModel, gyroRate.ViewModel, accel.ViewModel, orientation.ViewModel, storage);
            ViewModelStorage.StatusViewModel = vm;
            ;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
