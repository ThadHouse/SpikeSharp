using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.ViewModels;

namespace SpikeApp.Controls.Views
{
    public class ProgramViewer : UserControl
    {
        public ProgramViewer()
        {
            this.InitializeComponent();

            DataContext = ViewModelStorage.ProgramViewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
