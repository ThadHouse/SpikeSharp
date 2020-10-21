using System.IO;
using System.Threading.Tasks;

namespace SpikeLib
{
    public interface ISpikeConnection
    {
        Stream ReadStream { get; }
        Stream WriteStream { get; }
        Task CloseAsync();
    }
}
