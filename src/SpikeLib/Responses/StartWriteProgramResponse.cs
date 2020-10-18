using System;
using System.Text.Json;

namespace SpikeLib.Responses
{
    public class StartWriteProgramResponse : IResponse
    {
        public string Id { get; }

        public int  BlockSize { get; }
        public string TransferId { get; }

        public string RawText { get; }

        public StartWriteProgramResponse(string id, JsonDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            Id = id;
            RawText = document.RootElement.GetRawText();

            var properties = document.RootElement.GetProperty(stackalloc byte[] { (byte)'r' });

            BlockSize = properties.GetProperty("blocksize").GetInt32();
            TransferId = properties.GetProperty("transferid").GetString()!;
        }
    }
}
