using System;
using Letter.IO;
using Letter.Tcp;
using EventArgs = Letter.EventArgs;

namespace tcp_test1
{
    public class TcpTestChannel_Client : ITcpChannel
    {
        public TcpTestChannel_Client(string name)
        {
            this.name = name;
        }

        private string name;
        
        public void OnTransportActive(ITcpContext context)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_Client)}.{nameof(OnTransportActive)}");
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes("你好");
            context.WriteAsync(bytes, 0, bytes.Length);
        }

        public void OnTransportInactive(ITcpContext context)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_Client)}.{nameof(OnTransportInactive)}");
        }

        public void OnTransportException(ITcpContext context, Exception ex)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_Client)}.{nameof(OnTransportException)}>>{ex}");
        }

        public void OnTransportRead(ITcpContext context, ref WrappedStreamReader reader, ref EventArgs args)
        {
            // var span = args.buffer.First.Span;
            // string str = System.Text.Encoding.UTF8.GetString(span);
            
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_Client)}.{nameof(OnTransportRead)}>{args.buffer.Length}");
        }

        public void OnTransportWrite(ITcpContext context, ref WrappedStreamWriter writer, ref EventArgs args)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_Client)}.{nameof(OnTransportWrite)}");
        }
    }
}