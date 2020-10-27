using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp.Box
{
    public interface ITcpClientChannel : Bootstrap.IChannel
    {
        EndPoint ConnectAddress { get; }
        
        Task StartAsync(EndPoint address);
    }
}