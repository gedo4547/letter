using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System.Threading.Tasks;

namespace dotnetty_tcp_server
{
    class ServerHandler : ChannelHandlerAdapter
    {
        readonly IByteBuffer initialMessage;

        public ServerHandler()
        {
            this.initialMessage = Unpooled.Buffer(common.SocketConfig.message.Length);
            this.initialMessage.WriteBytes(common.SocketConfig.message);
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            System.Threading.Interlocked.Increment(ref ServerStatistics.client_count);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            System.Threading.Interlocked.Decrement(ref ServerStatistics.client_count);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            context.WriteAsync(initialMessage);
        }

        public override Task WriteAsync(IChannelHandlerContext context, object message)
        {
            return base.WriteAsync(context, message);
        }
    }
}
