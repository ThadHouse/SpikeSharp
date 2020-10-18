using SpikeLib.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpikeLib.Responces
{
    public class StorageResponse : IMessage
    {
        /*
         * {
         * "storage": {"available": 28504, "total": 31744, "pct": 11.2067, "unit": "kb", "free": 28504}, 
         * "slots": {"1": {"name": "VGhlIE1WUCBCdWdneQ==", "id": 52757, "project_id": "GqgkBnZ5kS8k", "modified": 1602972490188, "type": "scratch", "created": 1602968745287, "size": 2022}, "0": {"name": "U2NyYXRjaFllZXQ=", "id": 27602, "project_id": "KLRejmSIVdpK", "modified": 1602905610969, "type": "scratch", "created": 1602905225408, "size": 1863}, "5": {"name": "U29jY2VyIC0gcGVuYWx0eSBraWNr", "id": 31645, "project_id": "w6oOv2DmLVo1", "modified": 1602871063226, "type": "scratch", "created": 1602869979670, "size": 4075}, "2": {"name": "UHJvamVjdCAy", "id": 22622, "project_id": "1j82PE9b6ERw", "modified": 1602957212110, "type": "python", "created": 1602957200427, "size": 394}}}
        */
        private string rawString;
        public StorageResponse(JsonDocument document)
        {
            var properties = document.RootElement.GetProperty(stackalloc byte[] { (byte)'r' });
            rawString = properties.GetRawText();
        }

        public override string ToString()
        {
            return rawString;
        }
    }
}
