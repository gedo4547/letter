using System.Collections.Generic;
using System.Threading.Tasks;

namespace Letter
{
    public abstract class AStreamBootstrap<TOptions, TContext, TChannel> : ABootstrap<TOptions, ChannelGroupFactoryStreamImpl<TContext, TChannel>, ChannelGroupStreamImpl<TContext, TChannel>, TChannel, TContext>, IStreamBootstrap<TOptions, TContext, TChannel>
        where TOptions : IOptions
        where TContext : IContext
        where TChannel : IStreamChannel<TContext>
    {
        public AStreamBootstrap(TOptions options) : base(options)
        {
            this.channelGroupFactory = new ChannelGroupFactoryStreamImpl<TContext, TChannel>(this.OnCreateChannelGroup);
        }
        
        private ChannelGroupFactoryStreamImpl<TContext, TChannel> channelGroupFactory;
        protected override ChannelGroupFactoryStreamImpl<TContext, TChannel> ChannelGroupFactory
        {
            get { return this.channelGroupFactory; }
        }
        
        private ChannelGroupStreamImpl<TContext, TChannel> OnCreateChannelGroup(List<TChannel> arg)
        {
            return new ChannelGroupStreamImpl<TContext, TChannel>(arg);
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