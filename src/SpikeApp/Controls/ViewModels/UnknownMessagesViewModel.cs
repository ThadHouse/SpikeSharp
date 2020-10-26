using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using SpikeApp.Utilities;
using SpikeLib.Messages;

namespace SpikeApp.Controls.ViewModels
{
    public class UnknownMessagesViewModel : ViewModelBase
    {
        private string consoleContents = "";

        public string ConsoleLog
        {
            get => consoleContents;
            set => RaiseAndSetIfChanged(ref consoleContents, value);
        }

        private Task? channelReaderTask;

        private async Task ChannelReaderFuncAsync(ChannelReader<IMessage> reader)
        {
            while (true)
            {
                try
                {
                    var element = await reader.ReadAsync();
                    ConsoleLog += $"Type: {element.GetType()} RawMsg:\n {element.RawText}\n";
                }
                catch (ChannelClosedException)
                {
                    break;
                }
            }
        }

        public void AddChannelReader(ChannelReader<IMessage> reader)
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
