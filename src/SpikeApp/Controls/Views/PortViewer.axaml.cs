using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.ViewModels;

namespace SpikeApp.Controls.Views
{
    public class PortViewer : UserControl
    {
        private readonly PortViewerViewModel viewModel;

        public PortViewer()
        {
            viewModel = new PortViewerViewModel();
            this.InitializeComponent();

            DataContext = viewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public string Port
        {
            get => viewModel.Port;
            set => viewModel.Port = value;
        }
    }
}
