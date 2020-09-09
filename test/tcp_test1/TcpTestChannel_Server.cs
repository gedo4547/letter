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

        public void OnTransportActive(ITcpContext context)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_Server)}.{nameof(OnTransportActive)}");
        }

        public void OnTransportInactive(ITcpContext context)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_Server)}.{nameof(OnTransportInactive)}");
        }

        public void OnTransportException(ITcpContext context, Exception ex)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_Server)}.{nameof(OnTransportException)}>>{ex}");
        }

        public void OnTransportRead(ITcpContext context, ref WrappedStreamReader reader, ref EventArgs args)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_Server)}.{nameof(OnTransportRead)}>{args.buffer.Length}");
            var span = args.buffer.First.Span;
            string str = System.Text.Encoding.UTF8.GetString(span);
            Console.WriteLine(str);
        }

        public void OnTransportWrite(ITcpContext context, ref WrappedStreamWriter writer, ref EventArgs args)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_Server)}.{nameof(OnTransportWrite)}");
        }
    }
}