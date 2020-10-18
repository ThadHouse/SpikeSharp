using SpikeLib.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpikeLib.Responses
{
    public enum ProgramType
    {
        Scratch,
        Python,
        Unknown
    }

    public class StorageResponse : IResponse
    {
        public struct StorageStats
        {
            public int AvailableKb { get; }
            public int TotalKb { get; }
            public int FreeKb { get; }
            public float PercentageUsed { get; }

            public StorageStats(JsonElement element)
            {
                AvailableKb = element.GetProperty("available").GetInt32();
                TotalKb = element.GetProperty("total").GetInt32();
                FreeKb = element.GetProperty("free").GetInt32();
                PercentageUsed = element.GetProperty("pct").GetSingle();
            }
        }

        public struct SlotData
        {
            public SlotData(int slot, JsonElement element)
            {
                Slot = slot;
                Name = Encoding.UTF8.GetString(element.GetProperty("name").GetBytesFromBase64());
                Id = element.GetProperty("id").GetInt32();
                ProjectId = element.GetProperty("project_id").GetString()!;
                Modified = element.GetProperty("modified").GetInt64();
                var rawType = element.GetProperty("type").GetString()!;
                if (rawType == "scratch")
                {
                    Type = ProgramType.Scratch;
                }
                else if (rawType == "python")
                {
                    Type = ProgramType.Python;
                }
                else
                {
                    Type = ProgramType.Unknown;
                }
                Created = element.GetProperty("created").GetInt64();
                Size = element.GetProperty("size").GetInt32();
            }

            public int Slot { get; }
            public string Name { get; }
            public int Id { get; }
            public string ProjectId { get; }
            public long Modified { get; }
            public ProgramType Type { get; }
            public long Created { get; }
            public int Size { get; }
        }

        public StorageStats Storage { get; }

        public ReadOnlyMemory<SlotData> Slots { get; }

        public string Id { get; }

        /*
         * {
         * "storage": {"available": 28504, "total": 31744, "pct": 11.2067, "unit": "kb", "free": 28504}, 
         * "slots": {"1": {"name": "VGhlIE1WUCBCdWdneQ==", "id": 52757, "project_id": "GqgkBnZ5kS8k", "modified": 1602972490188, "type": "scratch", "created": 1602968745287, "size": 2022}, "0": {"name": "U2NyYXRjaFllZXQ=", "id": 27602, "project_id": "KLRejmSIVdpK", "modified": 1602905610969, "type": "scratch", "created": 1602905225408, "size": 1863}, "5": {"name": "U29jY2VyIC0gcGVuYWx0eSBraWNr", "id": 31645, "project_id": "w6oOv2DmLVo1", "modified": 1602871063226, "type": "scratch", "created": 1602869979670, "size": 4075}, "2": {"name": "UHJvamVjdCAy", "id": 22622, "project_id": "1j82PE9b6ERw", "modified": 1602957212110, "type": "python", "created": 1602957200427, "size": 394}}}
        */
        
        public string RawText { get; }

        public StorageResponse(string id, JsonDocument document)
        {
            var storageText = document.RootElement.GetRawText();
            Console.WriteLine(storageText);
            var properties = document.RootElement.GetProperty(stackalloc byte[] { (byte)'r' });
            var storage = properties.GetProperty("storage");
            Storage = new StorageStats(storage);

            var slots = properties.GetProperty("slots");

            SlotData[] slotsArray = new SlotData[20];
            int count = 0;
            foreach (var obj in slots.EnumerateObject())
            {
                slotsArray[count] = new SlotData(int.Parse(obj.Name), obj.Value);
                count++;
            }
            Slots = slotsArray.AsMemory().Slice(0, count);

            Id = id;

            RawText = properties.GetRawText();
        }
    }
}
