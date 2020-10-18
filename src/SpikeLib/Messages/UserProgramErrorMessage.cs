using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpikeLib.Messages
{
    public class UserProgramErrorMessage : IConsoleMessage
    {
        public string Backtrace { get; }
        public string CodeLocation { get; }

        public bool IsError => true;

        public UserProgramErrorMessage(JsonDocument document)
        {
            var properties = document.RootElement.GetProperty(stackalloc byte[] { (byte)'p' });

            var str = properties[3].GetString()!;
            var bse = Convert.FromBase64String(str);
            Backtrace = Encoding.UTF8.GetString(bse);

            str = properties[4].GetString()!;
            bse = Convert.FromBase64String(str);
            CodeLocation = Encoding.UTF8.GetString(bse);
            ;
        }

        public override string ToString()
        {
            return $"User Error:\n{Backtrace}{CodeLocation}";
        }
    }
}
