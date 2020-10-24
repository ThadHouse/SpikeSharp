using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Channels;
using System.Threading.Tasks;
using Avalonia.Controls;
using SpikeApp.Utilities;
using SpikeLib.Responses;

namespace SpikeApp.Controls.ViewModels
{
    public class ProgramViewerViewModel : ViewModelBase
    {
        public ObservableCollection<string> ProgramList { get; } = new();


        private void HandleUpdate(ReadOnlyMemory<SlotData> slots)
        {
            ProgramList.Clear();
            foreach (ref readonly var item in slots.Span)
            {
                ProgramList.Add($"{item.Slot}: {item.Name}");
            }
        }

        public async void Refresh()
        {
            await RefreshAsync();
        }

        public double Slot { get; set; }
        public string Name { get; set; } = "";


        public async Task RefreshAsync()
        {
            var hub = ViewModelStorage.Hub;
            if (hub == null) return;
            await Task.Delay(1200);
            await hub.RequestStorageAsync();
        }

        public async void UploadFile()
        {

            OpenFileDialog dialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Title = "Select File"
            };
            var result = await dialog.ShowAsync(ViewModelStorage.MainWindow);
            if (result == null || result.Length == 0) return;

            using Stream file = File.OpenRead(result[0]);

            var hub = ViewModelStorage.Hub;
            if (hub == null) return;
            if (string.IsNullOrWhiteSpace(Name)) return;
            await hub.UploadFileAsync(file, (int)Slot, Name);

        }

        private Task? channelReaderTask;

        private async Task ChannelReaderFuncAsync(ChannelReader<StorageResponse> reader)
        {
            while (true)
            {
                try
                {
                    var element = await reader.ReadAsync();
                    HandleUpdate(element.Slots);
                }
                catch (ChannelClosedException)
                {
                    break;
                }
            }
        }

        public void AddChannelReader(ChannelReader<StorageResponse> reader)
        {
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
    }
}
