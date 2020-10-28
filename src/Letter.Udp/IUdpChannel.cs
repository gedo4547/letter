using System.Net;
using System.Threading.Tasks;

namespace Letter.Udp
{
    public interface IUdpChannel : IChannel
    {
        EndPoint BindAddress { get; }

        Task StartAsync(EndPoint bindAddress);

        Task StartAsync(EndPoint bindAddress, EndPoint connectAddress);
    }
}