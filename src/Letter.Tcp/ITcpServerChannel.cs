using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public interface ITcpServerChannel : IChannel
    {
        EndPoint BindAddress { get; }

        Task StartAsync(EndPoint address);
    }
}