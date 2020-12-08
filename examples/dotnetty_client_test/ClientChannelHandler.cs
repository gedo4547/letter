using System;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace dotnetty_client_test
{
    public sealed class ClientChannelHandler : ChannelHandlerAdapter
    {
        private IChannelHandlerContext context;
        public long count;
        
        public override void ChannelActive(IChannelHandlerContext context)
        {
            this.context = context;
            Program.startSend += OnStartSend;
        }

        private void OnStartSend()
        {
            context.WriteAndFlushAsync(Buffer.messageBuffer);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
        }
        
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            this.count++;
            // Console.WriteLine("客户端收到");
            this.context.WriteAndFlushAsync(message as IByteBuffer);
           
        }
        
        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
    }
}