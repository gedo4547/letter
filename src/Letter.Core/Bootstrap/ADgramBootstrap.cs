using System.Collections.Generic;
using Letter.IO;

namespace Letter
{
    public abstract class ADgramBootstrap<TOptions, TContext> : ABootstrap<TOptions, ChannelGroupFactoryDgramImpl<TContext>, ChannelGroupDgramImpl<TContext>, IDgramChannel<TContext>, TContext, WrappedDgramReader, WrappedDgramWriter> 
        where TOptions: IOptions
        where TContext : class, IContext
    {
        public ADgramBootstrap()
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
    }
}