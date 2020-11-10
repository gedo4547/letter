using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Letter.IO;
using Letter.Udp;

namespace udp_text
{
    public class UdpFilter_2 : IUdpFilter
    {
        public void OnTransportActive(IUdpSession session)
        {
            Console.WriteLine($"{nameof(UdpFilter_2)}.{nameof(OnTransportActive)}" + session.LocalAddress);
            
            if (session.LocalAddress.ToString() == Program.c_p.ToString())
            {
                M.session = session;
                
                // string str = "nihao";
                //
                // Console.WriteLine("发送");
                // for (int i = 0; i < 20; i++)
                // {
                //     string tempStr = str + "__" + i.ToString();
                //     var arr = System.Text.Encoding.UTF8.GetBytes(tempStr);
                //     session.Write(Program.s_p, arr);
                //     await session.FlushAsync();
                //     
                //     System.Threading.Thread.Sleep(1000);
                //     
                // }
                
                
                
                // Task.Run(async ()=>{
                //     for (int i = 0; i < 20; i++)
                //     {
                //         string tempStr = str + "__" + i.ToString();
                //         var arr = System.Text.Encoding.UTF8.GetBytes(tempStr);
                //         session.Write(Program.s_p, arr);
                //         await session.FlushAsync();
                //
                //         await Task.Delay(1000);
                //     }
                // }).NoAwait();
            }
        }

        public void OnTransportInactive(IUdpSession session)
        {
            Console.WriteLine($"{nameof(UdpFilter_2)}.{nameof(OnTransportInactive)}");
        }

        public void OnTransportException(IUdpSession session, Exception ex)
        {
            Console.WriteLine($"{nameof(UdpFilter_2)}.{nameof(OnTransportException)}" + ex.ToString());
        }

        public void OnTransportRead(IUdpSession session, ref WrappedReader reader, WrappedArgs args)
        {
            long length = reader.Length;
            var buffer = reader.ReadBuffer((int)length);
            string str= System.Text.Encoding.UTF8.GetString(buffer.FirstSpan);

            Console.WriteLine($"{nameof(UdpFilter_2)}.{nameof(OnTransportRead)}>>LoaclAddress:{session.LocalAddress}    remoteAddress:{session.RcvAddress}>>" + str);
        }
        
        public void OnTransportWrite(IUdpSession session, ref WrappedWriter writer, WrappedArgs args)
        {
            Console.WriteLine($"{nameof(UdpFilter_2)}.{nameof(OnTransportWrite)}");
            var buffer = args.Value as byte[];
            
            Console.WriteLine(">>>>>>>"+(buffer.Length));
            
            writer.Write(buffer);
        }
    }
}