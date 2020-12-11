// using System.Threading.Tasks;
// using Letter.IO;
// using Letter.Udp;
//
// namespace Letter.Kcp
// {
//     abstract class AKcpBootstrap<TOptions, TChannel> : ABootstrap<TOptions, IKcpSession, TChannel>, IKcpBootstrap<TOptions, TChannel>
//         where TOptions : AKcpOptions, new()
//         where TChannel : IKcpChannel
//     {
//         public AKcpBootstrap()
//         {
//             this.udpBootstrap = UdpFactory.Bootstrap();
//             this.udpBootstrap.ConfigurationOptions(this.OnConfigurationOptions);
//             this.udpBootstrap.ConfigurationFilter(this.OnConfigurationFilter);
//         }
//         
//         protected IUdpBootstrap udpBootstrap;
//         
//         private void OnConfigurationOptions(UdpOptions options)
//         {
//             options.Order = this.options.Order;
//                 
//             options.RcvTimeout = this.options.RcvTimeout;
//             options.SndTimeout = this.options.SndTimeout;
//                 
//             options.RcvBufferSize = this.options.RcvBufferSize;
//             options.SndBufferSize = this.options.SndBufferSize;
//                 
//             options.SchedulerCount = this.options.SchedulerCount;
//             
//             options.MemoryPoolOptions = this.options.MemoryPoolOptions;
//         }
//         
//         private void OnConfigurationFilter(IFilterPipeline<IUdpSession> pipeline)
//         {
//             pipeline.Add(new DefaultBytesFilter());
//         }
//         
//         public override async Task BuildAsync()
//         {
//             await base.BuildAsync();
//             await this.udpBootstrap.BuildAsync();
//         }
//
//         public override async ValueTask DisposeAsync()
//         {
//             await base.DisposeAsync();
//             await this.udpBootstrap.DisposeAsync();
//         }
//     }
// }