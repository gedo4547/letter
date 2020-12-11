using System.Net;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Tcp
{
    public interface ITcpServerChannel : IChannel<ITcpSession, TcpServerOptions>
    {
        EndPoint BindAddress { get; }

        Task StartAsync(EndPoint address);
    }
}