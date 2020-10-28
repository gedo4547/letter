using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Letter.Bootstrap;

namespace Letter.Tcp.Box
{
    class TcpSession : ATcpSession
    {
        public TcpSession(Socket socket, ATcpOptions options, PipeScheduler scheduler, MemoryPool<byte> pool, ChannelFilterGroup<Box.ITcpSession, Box.ITcpChannelFilter> filterGroup) 
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