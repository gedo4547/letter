using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Letter.Bootstrap;

namespace Letter.Tcp.Box
{
    class TcpSslSession : ATcpSession
    {
        public TcpSslSession(Socket socket, ATcpOptions options, PipeScheduler scheduler, MemoryPool<byte> pool, SslFeature sslFeature, ChannelFilterGroup<Box.ITcpSession, Box.ITcpChannelFilter> filterGroup)
            : base(socket, options, scheduler, pool, filterGroup)
        {
        }

        public override Task StartAsync()
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteAsync(object obj)
        {
            throw new System.NotImplementedException();
        }
    }
}