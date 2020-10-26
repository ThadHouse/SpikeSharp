using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using SpikeApp.Utilities;
using SpikeLib.Messages;

namespace SpikeApp.Controls.ViewModels
{
    public class ConsoleControlViewModel : ViewModelBase
    {
        private string consoleContents = "";

        public string ConsoleLog
        {
            get => consoleContents;
            set => RaiseAndSetIfChanged(ref consoleContents, value);
        }

        private Task? channelReaderTask;

        private async Task ChannelReaderFuncAsync(ChannelReader<IConsoleMessage> reader)
        {
            while (true)
            {
                try
                {
                    var element = await reader.ReadAsync();
                    ConsoleLog += $"{element}";
                }
                catch (ChannelClosedException)
                {
                    break;
                }
            }
        }

        public void AddChannelReader(ChannelReader<IConsoleMessage> reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            channelReaderTask = ChannelReaderFuncAsync(reader);
        }

        public async Task RemoveChannelReaderAsync()
        {
            if (channelReaderTask != null)
            {
                await channelReaderTask;
            }
            channelReaderTask = null;
        }

        public void Clear()
        {
            ConsoleLog = "";
        }
    }
}
