using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpikeLib.Responses
{
    public class StartWriteProgramResponse : IResponse
    {
        public string Id { get; }

        public int  BlockSize { get; }
        public string TransferId { get; }

        public StartWriteProgramResponse(string id, JsonDocument document)
        {
            Id = id;

            var properties = document.RootElement.GetProperty(stackalloc byte[] { (byte)'r' });

            BlockSize = properties.GetProperty("blocksize").GetInt32();
            TransferId = properties.GetProperty("transferid").GetString()!;
        }
    }
}
