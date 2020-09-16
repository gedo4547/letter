using System;
using System.Buffers;
using System.Net;
using Letter;
using Letter.Udp;

namespace udp_text
{
    public class UdpFilter_2 : IUdpChannelFilter
    {
        public void OnChannelActive(IUdpSession session)
        {
            Console.WriteLine($"{nameof(UdpFilter_2)}.{nameof(OnChannelActive)}" + session.LoaclAddress);
            
            if (session.LoaclAddress.ToString() == Program.c_p.ToString())
            {
                string str = "nihao";
                var arr = System.Text.Encoding.UTF8.GetBytes(str);
                ReadOnlySequence<byte> sequence = new ReadOnlySequence<byte>(arr);
                
                Console.WriteLine("发送");
                session.WriteAsync(Program.s_p, ref sequence);
            }
        }

        public void OnChannelInactive(IUdpSession session)
        {
            Console.WriteLine($"{nameof(UdpFilter_2)}.{nameof(OnChannelInactive)}");
        }

        public void OnChannelException(IUdpSession session, Exception ex)
        {
            Console.WriteLine($"{nameof(UdpFilter_2)}.{nameof(OnChannelException)}" + ex.ToString());
        }

        public void OnChannelRead(IUdpSession session, ref WrappedDgramReader reader, ref ChannelArgs args)
        {
            int length = reader.Length;
            var buffer = reader.ReadRange(length);
            string str= System.Text.Encoding.UTF8.GetString(buffer.Span);

            Console.WriteLine($"{nameof(UdpFilter_2)}.{nameof(OnChannelRead)}>>LoaclAddress:{session.LoaclAddress}    remoteAddress:{session.RcvAddress}>>" + str);
        }

        public void OnChannelWrite(IUdpSession session, ref WrappedDgramWriter writer, ref ChannelArgs args)
        {
            Console.WriteLine($"{nameof(UdpFilter_2)}.{nameof(OnChannelWrite)}");
            writer.Write(ref args.buffer);
        }
    }
}