namespace SpikeLib.Messages
{
    public interface IConsoleMessage : IMessage
    {
        bool IsError { get; }
    }
}
