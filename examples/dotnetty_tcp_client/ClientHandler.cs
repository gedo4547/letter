using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Text;
using System.Threading.Tasks;

namespace dotnetty_tcp_client
{
    class ClientHandler : ChannelHandlerAdapter
    {
        private IByteBuffer initialMessage;
        private IChannelHandlerContext context;
        public long count;

        public ClientHandler()
        {
            Program.sendCallback += Send;
            
            this.initialMessage = Unpooled.Buffer(common.SocketConfig.message.Length);
            this.initialMessage.WriteBytes(common.SocketConfig.message);
        }

        private void Send()
        {
            context.WriteAndFlushAsync(this.initialMessage);
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            this.context = context;
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            //var buffer = context.Allocator.DirectBuffer();

            //var byteBuffer = message as IByteBuffer;
            //if (byteBuffer != null)
            //{
            //    Console.WriteLine("Received from server: " + byteBuffer.ToString(Encoding.UTF8));
            //}
            count++;
            context.WriteAsync(initialMessage);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();


        public override Task WriteAsync(IChannelHandlerContext context, object message)
        {
            return base.WriteAsync(context, message);
        }
    }
}
