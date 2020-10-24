using SpikeLib.Messages;

namespace SpikeApp.Controls.Status.Ports.ViewModels
{
    public interface IPortViewModel
    {
        string PortName { get; set; }
        void Update(in PortStatus status);
    }
}
