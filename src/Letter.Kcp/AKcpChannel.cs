using System;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Kcp
{
    abstract class AKcpChannel<TOptions> : AChannel<IKcpSession, TOptions>
        where TOptions : KcpOptions
    {
        public AKcpChannel(TOptions options, IKcpChannel channel, Action<IFilterPipeline<IKcpSession>> handler)
        {
            base.ConfigurationSelfOptions(options);
            base.ConfigurationSelfFilter(handler);

            this.channel = channel;
            this.channel.ConfigurationSelfFilter(base.handler);
        }

        protected IKcpChannel channel;

        public override async Task StopAsync()
        {
            await channel.StopAsync();
            await base.StopAsync();
        }
    }
}