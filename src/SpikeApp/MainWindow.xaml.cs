using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.ViewModels;

namespace SpikeApp
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            ViewModelStorage.MainWindow = this;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
