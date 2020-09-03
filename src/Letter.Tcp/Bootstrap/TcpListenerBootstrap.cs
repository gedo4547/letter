using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public class TcpListenerBootstrap : ATcpBootstrap<TcpListenerOptions>, ITcpListenerBootstrap
    {
        public override Task StartAsync(EndPoint point)
        {
            throw new System.NotImplementedException();
        }
    }
}