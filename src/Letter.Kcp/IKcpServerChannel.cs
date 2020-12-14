using System.Net;
using System.Threading.Tasks;

namespace Letter.Kcp
{
    public interface IKcpServerChannel : IKcpChannel<KcpServerOptions>
    {
        Task StartAsync(EndPoint address);
    }
}