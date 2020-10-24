using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.ViewModels;

namespace SpikeApp.Controls.Views
{
    public class SpikePortControl : UserControl
    {
        public SpikePortControl()
        {
            this.InitializeComponent();

            DataContext = ViewModelStorage.SpikePortViewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
