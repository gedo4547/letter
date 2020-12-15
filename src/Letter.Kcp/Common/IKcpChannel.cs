using System.Net;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Kcp
{
    public interface IKcpChannel : IChannel<IKcpSession, KcpOptions>
    {
        IKcpSession AddSession();

        bool RemoveSession(int num);
        
        Task BindAsync(EndPoint address);
    }
}