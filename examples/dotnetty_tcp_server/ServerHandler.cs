using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dotnetty_tcp_server
{
    class ServerHandler : ChannelHandlerAdapter
    {
        readonly IByteBuffer initialMessage;

        public ServerHandler()
        {
            this.initialMessage.WriteBytes(common.SocketConfig.message);
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            Console.WriteLine("发送");
            context.WriteAndFlushAsync(this.initialMessage);
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
