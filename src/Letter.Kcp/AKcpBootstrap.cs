using System.Threading.Tasks;

using Letter.IO;

namespace Letter.Kcp
{
    abstract class AKcpBootstrap<TOptions, TChannel> : ABootstrap<TOptions, IKcpSession, TChannel>, IKcpBootstrap<TOptions, TChannel>
        where TOptions : KcpOptions, new()
        where TChannel : IKcpChannel<TOptions>
    {
        public AKcpBootstrap()
        {
            this.kcpBootstrap = KcpFactory.Bootstrap();
            this.kcpBootstrap.ConfigurationGlobalOptions(this.OnConfigurationGlobalOptions);
            this.kcpBootstrap.ConfigurationGlobalFilter(this.OnConfigurationGlobalFilter);
        }

        protected IKcpBootstrap kcpBootstrap;

        private void OnConfigurationGlobalOptions(KcpOptions options)
        {
            options.Order = this.options.Order;

            options.RcvTimeout = this.options.RcvTimeout;
            options.SndTimeout = this.options.SndTimeout;

            options.RcvBufferSize = this.options.RcvBufferSize;
            options.SndBufferSize = this.options.SndBufferSize;

            options.SchedulerCount = this.options.SchedulerCount;

            options.MemoryPoolOptions = this.options.MemoryPoolOptions;
        }

        private void OnConfigurationGlobalFilter(IFilterPipeline<IKcpSession> pipeline)
        {
        }

        public override async Task BuildAsync()
        {
            await base.BuildAsync();
            await this.kcpBootstrap.BuildAsync();
        }

        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
            await this.kcpBootstrap.DisposeAsync();
        }
    }
}