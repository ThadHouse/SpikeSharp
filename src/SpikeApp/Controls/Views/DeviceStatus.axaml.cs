using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.ViewModels;

namespace SpikeApp.Controls.Views
{
    public class DeviceStatus : UserControl
    {
        public DeviceStatus()
        {
            this.InitializeComponent();

            DataContext = ViewModelStorage.StatusViewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
