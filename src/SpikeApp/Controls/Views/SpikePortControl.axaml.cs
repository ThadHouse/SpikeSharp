using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.ViewModels;

namespace SpikeApp.Controls.Views
{
    public class SpikePortControl : UserControl
    {
        private readonly SpikePortControlViewModel viewModel;

        public SpikePortControl()
        {
            this.InitializeComponent();

            viewModel = new SpikePortControlViewModel();
            DataContext = viewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
