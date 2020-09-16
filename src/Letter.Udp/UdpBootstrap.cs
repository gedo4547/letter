// using System.Threading.Tasks;
// using Letter;
//
// namespace Letter.Udp
// {
//     public class UdpBootstrap : ADgramBootstrap<UdpOptions, IUdpSession, IUdpFilter, IUdpNetwork>, IUdpBootstrap
//     {
//         protected override Task<IUdpNetwork> NetworkCreator()
//         {
//             IUdpNetwork network = new UdpNetwork(this.options);
//             return Task.FromResult(network);
//         }
//     }
// }