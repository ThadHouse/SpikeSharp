using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpikeLib;

namespace SpikeApp.Controls.ViewModels
{
    public static class ViewModelStorage
    {
        public static readonly ConsoleControlViewModel ConsoleViewModel = new();
        public static readonly SpikePortControlViewModel SpikePortViewModel = new();

        private static SpikeHub? Hub;

        public static async Task AddHubAsync(string comPort)
        {
            if (Hub != null)
            {
                await CloseHubAsync();
            }

            Hub = new SpikeHub(comPort);
            await Hub.OpenAsync();
            ConsoleViewModel.AddChannelReeader(Hub.ConsoleMessagesReader);
        }

        public static async Task CloseHubAsync()
        {
            if (Hub == null) return;

            await Hub.CloseAsync();
            await ConsoleViewModel.RemoveChannelReaderAsync();
            // Todo clear out readers
        }
    }
}
