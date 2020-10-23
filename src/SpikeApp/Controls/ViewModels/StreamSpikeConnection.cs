using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpikeLib;
using Windows.Devices.SerialCommunication;
using Windows.Networking.Sockets;

namespace SpikeApp.Controls.ViewModels
{
    public class StreamSpikeConnection : ISpikeConnection
    {
        private readonly StreamSocket device;

        public StreamSpikeConnection(StreamSocket device)
        {
            this.device = device;
        }

        public Stream ReadStream => device.InputStream.AsStreamForRead();

        public Stream WriteStream => device.OutputStream.AsStreamForWrite();

        public async Task CloseAsync()
        {
            await Task.Run(() =>
            {
                device.Dispose();
            });
        }
    }
}
