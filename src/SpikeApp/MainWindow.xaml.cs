using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SpikeApp
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}