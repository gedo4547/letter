using System;
using System.Buffers;
using Letter.IO;
using Letter.Tcp;
using EventArgs = Letter.EventArgs;

namespace tcp_test1
{
    public class TcpTestChannel_Server : ITcpChannel
    {
        public TcpTestChannel_Server(string name)
        {
            this.name = name;
        }

        private string name;

        public void OnChannelActive(ITcpContext context)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_Server)}.{nameof(OnChannelActive)}");
        }

        public void OnChannelInactive(ITcpContext context)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_Server)}.{nameof(OnChannelInactive)}");
        }

        public void OnChannelException(ITcpContext context, Exception ex)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_Server)}.{nameof(OnChannelException)}>>{ex}");
        }

        public void OnChannelRead(ITcpContext context, ref WrappedStreamReader reader, ref EventArgs args)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_Server)}.{nameof(OnChannelRead)}>{args.buffer.Length}");
            var span = args.buffer.First.Span;
            string str = System.Text.Encoding.UTF8.GetString(span);
            Console.WriteLine(str);
        }

        public void OnChannelWrite(ITcpContext context, ref WrappedStreamWriter writer, ref EventArgs args)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_Server)}.{nameof(OnChannelWrite)}");
        }
    }
}