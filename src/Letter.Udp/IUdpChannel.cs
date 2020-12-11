using System.Net;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Udp
{
    public interface IUdpChannel : IChannel<IUdpSession, UdpOptions>
    {
        EndPoint BindAddress { get; }

        Task StartAsync(EndPoint bindAddress);

        Task StartAsync(EndPoint bindAddress, EndPoint connectAddress);
    }
}