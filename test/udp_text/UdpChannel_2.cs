using System;
using Letter.IO;
using Letter.Udp;
using EventArgs = Letter.EventArgs;

namespace udp_text
{
    public class UdpChannel_2 : IUdpChannel
    {
        public UdpChannel_2(string name)
        {
            this.name = name;
        }

        private string name;

        public void OnChannelActive(IUdpContext context)
        {
            Console.WriteLine($"--{name}->{nameof(UdpChannel_2)}.{nameof(OnChannelActive)}");
        }

        public void OnChannelInactive(IUdpContext context)
        {
            Console.WriteLine($"--{name}->{nameof(UdpChannel_2)}.{nameof(OnChannelInactive)}");
        }

        public void OnChannelException(IUdpContext context, Exception ex)
        {
            Console.WriteLine($"--{name}->{nameof(UdpChannel_2)}.{nameof(OnChannelException)}"+ex.ToString());
        }

        public void OnChannelRead(IUdpContext context, ref WrappedDgramReader reader, ref EventArgs args)
        {
            int length = reader.Length;
            var buffer = reader.ReadRange(length);
            Console.WriteLine($"--{name}->{nameof(UdpChannel_2)}.{nameof(OnChannelRead)}");
            string str = System.Text.Encoding.UTF8.GetString(buffer.Span);
            Console.WriteLine(str);
        }

        public void OnChannelWrite(IUdpContext context, ref WrappedDgramWriter writer, ref EventArgs args)
        {
            Console.WriteLine($"--{name}->{nameof(UdpChannel_2)}.{nameof(OnChannelWrite)}");
            writer.Write(ref args.buffer);
        }
    }
}