using System.Net;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Kcp
{
    public interface IKcpChannel : IChannel<IKcpSession, KcpOptions>
    {
        bool Connect(uint conv, EndPoint remoteAddress);
        
        Task BindAsync(EndPoint address);
    }
}