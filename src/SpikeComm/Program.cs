using Newtonsoft.Json;
using SpikeLib;
using System;
using System.Threading.Tasks;

namespace SpikeComm
{
    internal class RpcData
    {
        [JsonProperty(PropertyName = "m")]
        public int Method { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            SpikeHub hub = new SpikeHub("COM8");
            await hub.OpenAsync();

            uint count = 0;

            while (true)
            {
                var val = await hub.ReadLine();
                if (val == null)
                {
                    return;
                }
                Console.WriteLine(val);
                ;
            }

        }
    }
}
