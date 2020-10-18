using SpikeLib.Messages;

namespace SpikeLib.Responses
{
    public interface IResponse : IMessage
    {
        string Id { get; }
    }
}
