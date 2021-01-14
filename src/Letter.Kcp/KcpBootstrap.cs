using System;
using System.Threading.Tasks;
using Letter.IO;
using Letter.Udp;

namespace Letter.Kcp
{
    sealed class KcpBootstrap : ABootstrap<KcpOptions, IKcpSession, IKcpChannel>, IKcpBootstrap
    {
        public KcpBootstrap()
        {
            this.udpBootstrap = UdpFactory.Bootstrap();
            this.udpBootstrap.ConfigurationGlobalOptions(this.OnConfigurationOptions);
        }

        private IKcpScheduler scheduler;
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

        public void ConfigurationGlobalThread(IKcpScheduler scheduler)
        {
            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }
            
            this.scheduler = scheduler;
        }
        
        public override async Task BuildAsync()
        {
            if (this.scheduler == null)
            {
                throw new Exception("The KCP thread must be configured");
            }

            await base.BuildAsync();
            await this.udpBootstrap.BuildAsync();
        }
        
        protected override async Task<IKcpChannel> ChannelFactoryAsync(KcpOptions options, Action<IFilterPipeline<IKcpSession>> handler)
        {
            var channel = await this.udpBootstrap.CreateChannelAsync();
            return new KcpChannel(options, channel, this.scheduler, handler); 
        }

        public override async ValueTask DisposeAsync()
        {
            await this.udpBootstrap.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}