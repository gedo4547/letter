using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;

using FilterGroup = Letter.StreamChannelFilterGroup<Letter.Tcp.ITcpSession, Letter.Tcp.ITcpChannelFilter>;

namespace Letter.Tcp
{
    class TcpSslSession : ATcpSession
    {
        public TcpSslSession(ITcpClient client, SslFeature sslFeature, FilterGroup filterGroup)
        {

        }


        public override Task StartAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}