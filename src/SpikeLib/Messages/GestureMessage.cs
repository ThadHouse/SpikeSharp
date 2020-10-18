using System;
using System.Text.Json;

namespace SpikeLib.Messages
{
    public enum Gesture
    {
        Tapped,
        Down,
        Front,
        Up,
        Back,
        Rightside,
        Leftside,
        Freefall,
        DoubleTapped,
        Shake
    }
    public class GestureMessage : IStatusMessage
    {
        public string RawText { get; }

        public GestureMessage(JsonDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            RawText = document.RootElement.GetRawText();
        }
    }
}
