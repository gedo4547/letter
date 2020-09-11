using System.Collections.Generic;
using System.Threading.Tasks;
using Letter.IO;
    
namespace Letter
{
    public abstract class AStreamNetwork<TOptions, TContext> :
        ANetwork<TOptions, ChannelGroupFactoryStreamImpl<TContext>, ChannelGroupStreamImpl<TContext>, IStreamChannel<TContext>, TContext, WrappedStreamReader, WrappedStreamWriter>,
        IStreamNetwork<TOptions, TContext>
    
        where TOptions : IOptions
        where TContext : class, IContext
    {
        public AStreamNetwork(TOptions options) : base(options)
        {
            this.channelGroupFactory = new ChannelGroupFactoryStreamImpl<TContext>(this.OnCreateChannelGroup);
        }
        
        private ChannelGroupFactoryStreamImpl<TContext> channelGroupFactory;
        protected override ChannelGroupFactoryStreamImpl<TContext> ChannelGroupFactory
        {
            get { return this.channelGroupFactory; }
        }
        
        private ChannelGroupStreamImpl<TContext> OnCreateChannelGroup(List<IStreamChannel<TContext>> arg)
        {
            return new ChannelGroupStreamImpl<TContext>(arg);
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