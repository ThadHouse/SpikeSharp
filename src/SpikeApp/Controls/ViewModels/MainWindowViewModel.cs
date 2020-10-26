using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpikeApp.Controls.Windows;
using SpikeApp.Utilities;

namespace SpikeApp.Controls.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private UnknownMessagesWindow? unknownWindow;
        private ConsoleMessagesWindow? consoleWindow;

        public void OpenConsoleViewer()
        {
            if (consoleWindow == null)
            {
                consoleWindow = new ConsoleMessagesWindow();
                consoleWindow.Closing += ConsoleWindow_Closing;
            }
            consoleWindow.Show();
        }

        private void ConsoleWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            consoleWindow = null;
        }

        public void OpenUnknownMessagesViewer()
        {
            if (unknownWindow == null)
            {
                unknownWindow = new UnknownMessagesWindow();
                unknownWindow.Closing += UnknownWindow_Closing;
            }
            unknownWindow.Show();
        }

#pragma warning disable CA1822 // Mark members as static
        public void Quit()
#pragma warning restore CA1822 // Mark members as static
        {
            ViewModelStorage.MainWindow.Close();
        }

        public void Close()
        {
            unknownWindow?.Close();
            consoleWindow?.Close();
        }

        private void UnknownWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            unknownWindow = null;
        }
    }
}
