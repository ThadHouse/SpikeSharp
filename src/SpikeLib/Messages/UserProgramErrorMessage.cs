using System;
using System.Text;
using System.Text.Json;

namespace SpikeLib.Messages
{
    public class UserProgramErrorMessage : IConsoleMessage
    {
        public string Backtrace { get; }
        public string CodeLocation { get; }

        public bool IsError => true;

        public string RawText { get; }

        public UserProgramErrorMessage(JsonDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            RawText = document.RootElement.GetRawText();
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
