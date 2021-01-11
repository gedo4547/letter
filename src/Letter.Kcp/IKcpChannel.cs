using System.Net;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Kcp
{
    public interface IKcpChannel : IChannel<IKcpSession, KcpOptions>
    {
        void ConfigurationSelfController(AKcpController controller);

        Task BindAsync(EndPoint address);
    }
}