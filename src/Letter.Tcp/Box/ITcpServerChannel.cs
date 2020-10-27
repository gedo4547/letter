using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp.Box
{
    public interface ITcpServerChannel : Bootstrap.IChannel
    {
        EndPoint BindAddress { get; }

        Task StartAsync(EndPoint address);
    }
}