// using System;
// using System.Buffers;
// using System.Net;
// using System.Threading.Tasks;
// using Letter;
// using Letter.Udp;
//
// namespace udp_text
// {
//     public class UdpFilter_1 : IUdpFilter
//     {
//         public UdpFilter_1(string name)
//         {
//             this.name = name;
//         }
//
//         private string name;
//
//         public void OnFilterActive(IUdpContext context)
//         { 
//             Console.WriteLine($"--{name}->{nameof(UdpFilter_1)}.{nameof(OnFilterActive)}");
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
//         public void OnFilterInactive(IUdpContext context)
//         {
//             Console.WriteLine($"--{name}->{nameof(UdpFilter_1)}.{nameof(OnFilterInactive)}");
//         }
//
//         public void OnFilterException(IUdpContext context, Exception ex)
//         {
//             Console.WriteLine($"--{name}->{nameof(UdpFilter_1)}.{nameof(OnFilterException)}>>"+ex.ToString());
//         }
//
//         public void OnFilterRead(IUdpContext context, ref WrappedDgramReader reader, ref FilterArgs args)
//         {
//             Console.WriteLine($"--{name}->{nameof(UdpFilter_1)}.{nameof(OnFilterRead)}");
//         }
//
//         public void OnFilterWrite(IUdpContext context, ref WrappedDgramWriter writer, ref FilterArgs args)
//         {
//             Console.WriteLine($"--{name}->{nameof(UdpFilter_1)}.{nameof(OnFilterWrite)}");
//             writer.Write(ref args.buffer);
//         }
//     }
// }