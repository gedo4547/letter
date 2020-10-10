using System;
using System.Buffers;
using Letter;
using Letter.Udp;

namespace udp_text
{
    public class UdpFilter_2 : IUdpChannelFilter
    {
        public async void OnChannelActive(IUdpSession session)
        {
            Console.WriteLine($"{nameof(UdpFilter_2)}.{nameof(OnChannelActive)}" + session.LoaclAddress);
            
            if (session.LoaclAddress.ToString() == Program.c_p.ToString())
            {
                string str = "nihao";
                
                Console.WriteLine("发送");
                for (int i = 0; i < 8; i++)
                {
                    string tempStr = str + "__" + i.ToString();
                    var arr = System.Text.Encoding.UTF8.GetBytes(tempStr);
                    ReadOnlySequence<byte> sequence = new ReadOnlySequence<byte>(arr);
                    await session.WriteAsync(Program.s_p, ref sequence);
                }
                
                Console.WriteLine("关闭");
                 await session.DisposeAsync();
                // await Task.Run(async () =>
                // {
                //     await Task.Delay(3 * 1000);
                //     Console.WriteLine("关闭");    
                //     await session.DisposeAsync();
                // });
                //
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
            var buffer = args.buffers[0];
            writer.Write(ref buffer);
        }
    }
}