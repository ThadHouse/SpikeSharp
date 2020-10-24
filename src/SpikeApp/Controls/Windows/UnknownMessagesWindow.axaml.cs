using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.ViewModels;

namespace SpikeApp.Controls.Windows
{
    public class UnknownMessagesWindow : Window
    {
        public UnknownMessagesWindow()
        {
            DataContext = ViewModelStorage.UnknownViewModel;
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
