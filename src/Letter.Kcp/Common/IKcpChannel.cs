using System.Net;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Kcp
{
    public interface IKcpChannel : IChannel<IKcpSession, KcpOptions>
    {
        IKcpSession CreateSession(EndPoint remoteAddress);
        
        Task BindAsync(EndPoint address);
    }
}