using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using SpikeLib;

namespace SpikeApp.Controls.ViewModels
{
    public static class ViewModelStorage
    {
        public static readonly ConsoleControlViewModel ConsoleViewModel = new();
        public static readonly SpikePortControlViewModel SpikePortViewModel = new();
        public static readonly ProgramViewerViewModel ProgramViewModel = new();

        public static SpikeHub? Hub;

        public static Window MainWindow = null!;

        public static async Task AddHubAsync(string comPort)
        {
            if (Hub != null)
            {
                await CloseHubAsync();
            }

            Hub = new SpikeHub(comPort);
            await Hub.OpenAsync();
            ConsoleViewModel.AddChannelReeader(Hub.ConsoleMessagesReader);
            ProgramViewModel.AddChannelReeader(Hub.StorageUpdateReader);
            await ProgramViewModel.RefreshAsync();
        }

        public static async Task CloseHubAsync()
        {
            if (Hub == null) return;

            await Hub.CloseAsync();
            await ConsoleViewModel.RemoveChannelReaderAsync();
            await ProgramViewModel.RemoveChannelReaderAsync();
            // Todo clear out readers
        }
    }
}
