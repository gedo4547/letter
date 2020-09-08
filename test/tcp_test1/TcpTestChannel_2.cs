using System;
using Letter.IO;
using Letter.Tcp;
using EventArgs = Letter.EventArgs;

namespace tcp_test1
{
    public class TcpTestChannel_2 : ITcpChannel
    {
        public TcpTestChannel_2(string name)
        {
            this.name = name;
        }

        private string name;

        public void OnTransportActive(ITcpContext context)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_2)}.{nameof(OnTransportActive)}");
        }

        public void OnTransportInactive(ITcpContext context)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_2)}.{nameof(OnTransportInactive)}");
        }

        public void OnTransportException(ITcpContext context, Exception ex)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_2)}.{nameof(OnTransportException)}");
        }

        public void OnTransportRead(ITcpContext context, ref WrappedStreamReader reader, ref EventArgs args)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_2)}.{nameof(OnTransportRead)}");
        }

        public void OnTransportWrite(ITcpContext context, ref WrappedStreamWriter writer, ref EventArgs args)
        {
            Console.WriteLine($"--{name}->{nameof(TcpTestChannel_2)}.{nameof(OnTransportWrite)}");
        }
    }
}