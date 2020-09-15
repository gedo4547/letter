// using System;
// using System.Buffers;
// using System.Net;
// using System.Threading.Tasks;
// using Letter;
// using Letter.Udp;
//
// namespace udp_text
// {
//     public class UdpChannel_1 : IUdpChannel
//     {
//         public UdpChannel_1(string name)
//         {
//             this.name = name;
//         }
//
//         private string name;
//
//         public void OnChannelActive(IUdpContext context)
//         { 
//             Console.WriteLine($"--{name}->{nameof(UdpChannel_1)}.{nameof(OnChannelActive)}");
//             Task.Run(async () =>
//             {
//                 await Task.Delay(5 * 1000);
//                 string str = "你好";
//                 var buffer = System.Text.Encoding.UTF8.GetBytes(str);
//                 ReadOnlySequence<byte> readOnlySequence = new ReadOnlySequence<byte>(buffer);
//                 await context.WriteAsync(new IPEndPoint(IPAddress.Loopback, 20002), ref readOnlySequence);
//             });
//             
//             
//         }
//
//         public void OnChannelInactive(IUdpContext context)
//         {
//             Console.WriteLine($"--{name}->{nameof(UdpChannel_1)}.{nameof(OnChannelInactive)}");
//         }
//
//         public void OnChannelException(IUdpContext context, Exception ex)
//         {
//             Console.WriteLine($"--{name}->{nameof(UdpChannel_1)}.{nameof(OnChannelException)}>>"+ex.ToString());
//         }
//
//         public void OnChannelRead(IUdpContext context, ref WrappedDgramReader reader, ref ChannelArgs args)
//         {
//             Console.WriteLine($"--{name}->{nameof(UdpChannel_1)}.{nameof(OnChannelRead)}");
//         }
//
//         public void OnChannelWrite(IUdpContext context, ref WrappedDgramWriter writer, ref ChannelArgs args)
//         {
//             Console.WriteLine($"--{name}->{nameof(UdpChannel_1)}.{nameof(OnChannelWrite)}");
//             writer.Write(ref args.buffer);
//         }
//     }
// }