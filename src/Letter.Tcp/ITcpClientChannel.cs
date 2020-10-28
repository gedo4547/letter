using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public interface ITcpClientChannel : IChannel
    {
        EndPoint ConnectAddress { get; }
        
        Task StartAsync(EndPoint address);
    }
}