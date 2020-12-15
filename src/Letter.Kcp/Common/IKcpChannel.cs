using System.Net;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Kcp
{
    public interface IKcpChannel<TOptions> : IChannel<IKcpSession, TOptions>
        where TOptions : IOptions
    {
        Task BindAsync(EndPoint address);
    }
}