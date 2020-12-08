using System;
using common;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace dotnetty_server_test
{
    public sealed class ServerChannelHandler : ChannelHandlerAdapter
    {
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
            // Console.WriteLine("服务端收到");
            context.WriteAndFlushAsync(message as IByteBuffer);
        }
        
        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
    }
}