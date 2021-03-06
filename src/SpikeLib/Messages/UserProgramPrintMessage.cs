﻿using System;
using System.Text;
using System.Text.Json;

namespace SpikeLib.Messages
{
    public class UserProgramPrintMessage : IConsoleMessage
    {
        public string Message { get; }

        public bool IsError => false;

        public string RawText { get; }

        public UserProgramPrintMessage(JsonDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            RawText = document.RootElement.GetRawText();
            var properties = document.RootElement.GetProperty(stackalloc byte[] { (byte)'p' });

            var b64 = properties.GetProperty("value").GetString()!;
            if (b64.EndsWith('\n')) 
                b64 = b64.Substring(0, b64.Length - 1);
            Message = Encoding.UTF8.GetString(Convert.FromBase64String(b64));
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
