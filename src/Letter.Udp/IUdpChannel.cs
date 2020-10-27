using System.Net;
using System.Threading.Tasks;

namespace Letter.Udp.Box
{
    public interface IUdpChannel : Letter.Bootstrap.IChannel
    {
        EndPoint BindAddress { get; }

        Task StartAsync(EndPoint bindAddress);

        Task StartAsync(EndPoint bindAddress, EndPoint connectAddress);
    }
}