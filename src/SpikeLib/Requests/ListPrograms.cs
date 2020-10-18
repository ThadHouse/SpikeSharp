using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpikeLib.Requests
{
    public class ListPrograms : IRequest
    {
        public char WriteJson(Utf8JsonWriter writer)
        {
            writer.WriteString("m", "get_storage_status");
            writer.WriteStartObject("p");
            writer.WriteEndObject();
            return '0';
        }
    }
}
