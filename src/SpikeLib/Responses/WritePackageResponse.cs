using System.Text.Json;

namespace SpikeLib.Responses
{
    class WritePackageResponse : IResponse
    {
        public string Id { get; }

        public string MessageData { get; }

        public string RawText { get; }

        public WritePackageResponse(string id, JsonDocument document)
        {
            Id = id;
            RawText = document.RootElement.GetRawText();

            MessageData = document.RootElement.GetRawText();
        }
    }
}
