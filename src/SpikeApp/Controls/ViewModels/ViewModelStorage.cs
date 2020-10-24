using System.Threading.Tasks;
using Avalonia.Controls;
using SpikeApp.Controls.Status.ViewModels;
using SpikeApp.Controls.Windows;
using SpikeLib;

namespace SpikeApp.Controls.ViewModels
{
    public static class ViewModelStorage
    {
        public static readonly ConsoleControlViewModel ConsoleViewModel = new();
        public static readonly SpikePortControlViewModel SpikePortViewModel = new();
        public static readonly ProgramViewerViewModel ProgramViewModel = new();
        public static readonly UnknownMessagesViewModel UnknownViewModel = new();

#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static SpikeHub? Hub;
        public static Window MainWindow = null!;
        public static DeviceStatusViewModel StatusViewModel = null!;
#pragma warning restore CA2211 // Non-constant fields should not be visible

        public static async Task AddHubAsync(ISpikeConnection spikeConnection)
        {
            if (Hub != null)
            {
                await CloseHubAsync();
            }

            Hub = new SpikeHub(spikeConnection);
            ConsoleViewModel.AddChannelReader(Hub.ConsoleMessagesReader);
            ProgramViewModel.AddChannelReader(Hub.StorageUpdateReader);
            StatusViewModel.AddChannelReader(Hub.StatusMessageReader);
            UnknownViewModel.AddChannelReader(Hub.UnknownMessagesReader);
            await Task.Delay(500);
            await ProgramViewModel.RefreshAsync();
        }

        public static async Task CloseHubAsync()
        {
            if (Hub == null) return;

            await Hub.CloseAsync();
            await ConsoleViewModel.RemoveChannelReaderAsync();
            await ProgramViewModel.RemoveChannelReaderAsync();
            await StatusViewModel.RemoveChannelReaderAsync();
            await UnknownViewModel.RemoveChannelReaderAsync();
            // Todo clear out readers
        }

        private static UnknownMessagesWindow? unknownWindow;

        public static void StartUnknownWindow()
        {
            if (unknownWindow == null)
            {
                unknownWindow = new UnknownMessagesWindow();
                unknownWindow.Closing += UnknownWindow_Closing;
            }
            unknownWindow.Show();
        }

        private static void UnknownWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            unknownWindow = null;
        }
    }
}
