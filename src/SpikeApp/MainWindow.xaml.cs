using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpikeApp.Controls.ViewModels;

namespace SpikeApp
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            ViewModelStorage.MainWindow = this;
            InitializeComponent();

            if (Properties.WindowSettings.Default.WindowHeight >= this.MinHeight)
            {
                Height = Properties.WindowSettings.Default.WindowHeight;
            }

            if (Properties.WindowSettings.Default.WindowWidth >= this.MinWidth)
            {
                Width = Properties.WindowSettings.Default.WindowWidth;
            }

            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.WindowSettings.Default.WindowHeight = this.Height;
            Properties.WindowSettings.Default.WindowWidth = this.Width;
            Properties.WindowSettings.Default.Save();
            ViewModelStorage.CloseUnknownWindow();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
