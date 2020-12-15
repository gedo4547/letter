using System;
using System.Threading.Tasks;
using Letter.IO;
using Letter.Udp;

namespace Letter.Kcp
{
    sealed class KcpBootstrap<TOptions, TChannel> : ABootstrap<TOptions, IKcpSession, TChannel>, IKcpBootstrap<TOptions, TChannel>
        where TOptions : KcpOptions, new()
        where TChannel : IKcpChannel<TOptions>
    {
        public KcpBootstrap()
        {
            this.udpBootstrap = UdpFactory.Bootstrap();
            this.udpBootstrap.ConfigurationGlobalOptions(this.OnConfigurationOptions);
            this.udpBootstrap.ConfigurationGlobalFilter(this.OnConfigurationFilter);
        }

        private IUdpBootstrap udpBootstrap; 

        private void OnConfigurationOptions(UdpOptions options)
        {
            options.Order = this.options.Order;
            options.RcvTimeout = this.options.RcvTimeout;
            options.SndTimeout = this.options.SndTimeout;
            options.RcvBufferSize = this.options.RcvBufferSize;
            options.SndBufferSize = this.options.SndBufferSize;
            options.SchedulerCount = this.options.SchedulerCount;
            options.MemoryPoolOptions = this.options.MemoryPoolOptions;
        }
        
        private void OnConfigurationFilter(IFilterPipeline<IUdpSession> pipeline)
        {
            pipeline.Add(new DefaultBytesFilter());
        }
        
        public override async Task BuildAsync()
        {
            await base.BuildAsync();
            await this.udpBootstrap.BuildAsync();
        }
        
        protected override Task<TChannel> ChannelFactoryAsync(TOptions options, Action<IFilterPipeline<IKcpSession>> handler)
        {
            var channel = this.udpBootstrap.CreateAsync();
            throw new NotImplementedException();
        }

        public override async ValueTask DisposeAsync()
        {
            await this.udpBootstrap.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}