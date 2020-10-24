using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.Status.ViewModels;

namespace SpikeApp.Controls.Status.Views
{
    public class BatteryView : UserControl
    {
        public BatteryViewModel ViewModel { get; } = new();

        public BatteryView()
        {
            DataContext = ViewModel;
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
