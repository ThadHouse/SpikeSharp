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

        public int Slot { get; set; }

        private bool runOnDeploy = true;
        public bool RunOnDeploy
        {
            get => runOnDeploy;
            set => RaiseAndSetIfChanged(ref runOnDeploy, value);
        }

        private bool compile = true;
        public bool Compile
        {
            get => compile;
            set => RaiseAndSetIfChanged(ref compile, value);
        }

#pragma warning disable CA1822 // Mark members as static
        public async Task RefreshAsync()
#pragma warning restore CA1822 // Mark members as static
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
            int slot = Slot;
            // TODO Check for closed channel in chain
            await hub.UploadFileAsync(file, slot, Path.GetFileNameWithoutExtension(result[0]));
            if (RunOnDeploy)
            {
                await hub.RunProgramAsync(slot);
            }

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
    }
}
