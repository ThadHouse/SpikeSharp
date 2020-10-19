using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.ViewModels;

namespace SpikeApp.Controls.Views
{
    public class ConsoleControl : UserControl
    {
        public ConsoleControl()
        {
            this.InitializeComponent();

            DataContext = ViewModelStorage.ConsoleViewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
