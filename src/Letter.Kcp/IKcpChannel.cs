using System.Net;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Kcp
{
    public interface IKcpChannel : IChannel<IKcpSession, KcpOptions>
    {
        TController BindSelfController<TController>(TController controller) where TController : AKcpController;

        Task BindAsync(EndPoint address);
    }
}