using System.Net;
using System.Threading.Tasks;

namespace Letter.Kcp
{
    public interface IKcpClientChannel : IKcpChannel<KcpClientOptions>
    {
        Task StartAsync(EndPoint address);
    }
}