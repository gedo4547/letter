using System.Net;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Kcp
{
    class KcpServerChannel : AChannel<IKcpSession, KcpServerOptions>, IKcpServerChannel
    {
        public Task StartAsync(EndPoint address)
        {
            throw new System.NotImplementedException();
        }
    }
}