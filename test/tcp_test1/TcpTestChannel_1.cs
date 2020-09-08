using System;
using Letter.IO;
using Letter.Tcp;
using EventArgs = Letter.EventArgs;

namespace tcp_test1
{
    public class TcpTestChannel_1 : ITcpChannel
    {
        public TcpTestChannel_1(string name)
        {
            this.name = name;
        }

        private string name;
        
        public void OnTransportActive(ITcpContext context)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_1)}.{nameof(OnTransportActive)}");
        }

        public void OnTransportInactive(ITcpContext context)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_1)}.{nameof(OnTransportInactive)}");
        }

        public void OnTransportException(ITcpContext context, Exception ex)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_1)}.{nameof(OnTransportException)}");
        }

        public void OnTransportRead(ITcpContext context, ref WrappedStreamReader reader, ref EventArgs args)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_1)}.{nameof(OnTransportRead)}");
        }

        public void OnTransportWrite(ITcpContext context, ref WrappedStreamWriter writer, ref EventArgs args)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_1)}.{nameof(OnTransportWrite)}");
        }
    }
}