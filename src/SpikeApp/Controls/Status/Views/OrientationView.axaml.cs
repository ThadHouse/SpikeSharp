using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.Status.ViewModels;

namespace SpikeApp.Controls.Status.Views
{
    public class OrientationView : UserControl
    {
        public OrientationViewModel ViewModel { get; } = new();
        public OrientationView()
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
