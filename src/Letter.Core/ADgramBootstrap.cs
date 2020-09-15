using System.Collections.Generic;
using System.Threading.Tasks;

namespace Letter
{
    public abstract class ADgramBootstrap<TOptions, TContext, TChannel, TNetwork> : ABootstrap<TOptions, ChannelGroupFactoryDgramImpl<TContext, TChannel>, ChannelGroupDgramImpl<TContext, TChannel>, TChannel, TContext, TNetwork>, IDgramBootstrap<TOptions, TContext, TChannel>
        where TChannel : IDgramChannel<TContext>
        where TOptions : IOptions
        where TContext : IContext
        where TNetwork : IDgramNetwork<TOptions, TChannel, TContext>
    {
        public ADgramBootstrap(TOptions options) : base(options)
        {
            this.channelGroupFactory = new ChannelGroupFactoryDgramImpl<TContext, TChannel>(this.OnCreateChannelGroup);
        }

        private ChannelGroupFactoryDgramImpl<TContext, TChannel> channelGroupFactory;
        protected override ChannelGroupFactoryDgramImpl<TContext, TChannel> ChannelGroupFactory
        {
            get { return this.channelGroupFactory; }
        }

        public ChannelGroupDgramImpl<TContext, TChannel> OnCreateChannelGroup(List<TChannel> arg)
        {
            return new ChannelGroupDgramImpl<TContext, TChannel>(arg);
        }
        
        public override ValueTask DisposeAsync()
        {
            if (this.channelGroupFactory != null)
            {
                this.channelGroupFactory.Dispose();
                this.channelGroupFactory = null;
            }
            
            return base.DisposeAsync();
        }
    }
}