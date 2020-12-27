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

        private IUdpBootstrap udpBootstrap;
        private IKcpThread thread;

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

        public void ConfigurationGlobalThread(IKcpThread thread)
        {
            if (thread == null)
            {
                throw new ArgumentNullException(nameof(thread));
            }
            
            this.thread = thread;
        }
        
        public override async Task BuildAsync()
        {
            await base.BuildAsync();
            await this.udpBootstrap.BuildAsync();
        }
        
        protected override async Task<IKcpChannel> ChannelFactoryAsync(KcpOptions options, Action<IFilterPipeline<IKcpSession>> handler)
        {
            var channel = await this.udpBootstrap.CreateAsync();
            return new KcpChannel(options, channel, this.thread, handler); 
        }

        public override async ValueTask DisposeAsync()
        {
            await this.udpBootstrap.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}