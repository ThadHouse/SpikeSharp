using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SpikeApp.Controls.Views
{
    public class DeviceStatus : UserControl
    {
        public DeviceStatus()
        {
            this.InitializeComponent();

            //DataContext = ViewModelStorage.StatusViewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
