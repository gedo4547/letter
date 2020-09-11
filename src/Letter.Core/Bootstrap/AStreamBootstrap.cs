using System.Collections.Generic;
using Letter.IO;
    
    namespace Letter
    {
        public abstract class AStreamBootstrap<TOptions, TContext> : ABootstrap<TOptions, ChannelGroupFactoryStreamImpl<TContext>, ChannelGroupStreamImpl<TContext>, IStreamChannel<TContext>, TContext, WrappedStreamReader, WrappedStreamWriter> 
            where TOptions: IOptions
            where TContext : class, IContext
        {
            public AStreamBootstrap()
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
        }
    }