using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.ViewModels;

namespace SpikeApp.Controls.Windows
{
    public class ConsoleMessagesWindow : Window
    {
        public ConsoleMessagesWindow()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
