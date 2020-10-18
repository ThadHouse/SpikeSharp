using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SpikeApp.Controls.Views
{
    public class SpikePortControl : UserControl
    {
        public SpikePortControl()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
