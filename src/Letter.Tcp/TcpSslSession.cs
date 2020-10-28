using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace Letter.Tcp
{
    class TcpSslSession : ATcpSession
    {
        public TcpSslSession(Socket socket, ATcpOptions options, PipeScheduler scheduler, MemoryPool<byte> pool, SslFeature sslFeature, ChannelFilterGroup<ITcpSession, ITcpChannelFilter> filterGroup)
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