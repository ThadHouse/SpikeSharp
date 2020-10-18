using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpikeLib.Requests
{
    public interface IRequest
    {
        char WriteJson(Utf8JsonWriter writer);
    }
}
