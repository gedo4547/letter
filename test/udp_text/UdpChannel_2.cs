// using System;
// using Letter;
// using Letter.Udp;
//
// namespace udp_text
// {
//     public class UdpFilter_2 : IUdpFilter
//     {
//         public UdpFilter_2(string name)
//         {
//             this.name = name;
//         }
//
//         private string name;
//
//         public void OnFilterActive(IUdpContext context)
//         {
//             Console.WriteLine($"--{name}->{nameof(UdpFilter_2)}.{nameof(OnFilterActive)}");
//         }
//
//         public void OnFilterInactive(IUdpContext context)
//         {
//             Console.WriteLine($"--{name}->{nameof(UdpFilter_2)}.{nameof(OnFilterInactive)}");
//         }
//
//         public void OnFilterException(IUdpContext context, Exception ex)
//         {
//             Console.WriteLine($"--{name}->{nameof(UdpFilter_2)}.{nameof(OnFilterException)}"+ex.ToString());
//         }
//
//         public void OnFilterRead(IUdpContext context, ref WrappedDgramReader reader, ref FilterArgs args)
//         {
//             int length = reader.Length;
//             var buffer = reader.ReadRange(length);
//             Console.WriteLine($"--{name}->{nameof(UdpFilter_2)}.{nameof(OnFilterRead)}");
//             string str = System.Text.Encoding.UTF8.GetString(buffer.Span);
//             Console.WriteLine(str);
//         }
//
//         public void OnFilterWrite(IUdpContext context, ref WrappedDgramWriter writer, ref FilterArgs args)
//         {
//             Console.WriteLine($"--{name}->{nameof(UdpFilter_2)}.{nameof(OnFilterWrite)}");
//             writer.Write(ref args.buffer);
//         }
//     }
// }