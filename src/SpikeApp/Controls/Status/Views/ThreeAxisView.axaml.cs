using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.Status.ViewModels;

namespace SpikeApp.Controls.Status.Views
{
    public class ThreeAxisView : UserControl
    {
        public ThreeAxisViewModel ViewModel { get; } = new ThreeAxisViewModel();

        public ThreeAxisView()
        {
            DataContext = ViewModel;
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public string TypeName
        {
            get => ViewModel.TypeName;
            set => ViewModel.TypeName = value;
        }
    }
}
