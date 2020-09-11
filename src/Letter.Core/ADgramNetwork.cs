using System.Collections.Generic;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter
{
    public abstract class ADgramNetwork<TOptions, TContext> : 
        ANetwork<TOptions, ChannelGroupFactoryDgramImpl<TContext>, ChannelGroupDgramImpl<TContext>, IDgramChannel<TContext>, TContext, WrappedDgramReader, WrappedDgramWriter>,
        IDgramNetwork<TOptions, TContext>
    
        where TOptions: IOptions
        where TContext : class, IContext
    {
        public ADgramNetwork()
        {
            this.channelGroupFactory = new ChannelGroupFactoryDgramImpl<TContext>(this.OnCreateChannelGroup);
        }

        private ChannelGroupFactoryDgramImpl<TContext> channelGroupFactory;
        protected override ChannelGroupFactoryDgramImpl<TContext> ChannelGroupFactory
        {
            get { return this.channelGroupFactory; }
        }

        public ChannelGroupDgramImpl<TContext> OnCreateChannelGroup(List<IDgramChannel<TContext>> arg)
        {
            return new ChannelGroupDgramImpl<TContext>(arg);
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