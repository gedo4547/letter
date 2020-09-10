using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Letter
{
    public abstract class ABootstrap<TOptions, TChannelGroup, TChannel, TContext, TReader, TWriter> : IBootstrap<TOptions, TChannel, TContext, TReader, TWriter>
        where TOptions: IOptions
        where TReader : struct
        where TWriter : struct
        where TContext : class, IContext
        where TChannel : IChannel<TContext, TReader, TWriter>
        where TChannelGroup : AChannelGroup<TChannel, TContext, TReader, TWriter>
    {
        public ABootstrap()
        {
            this.channelGroupFactory = new ChannelGroupFactory<TChannelGroup, TChannel, TContext, TReader, TWriter>(this.OnCreateChannelGroup);
        }
        
        protected Action<TOptions> optionsFactory;
        protected ChannelGroupFactory<TChannelGroup, TChannel, TContext, TReader, TWriter> channelGroupFactory;
        
        public void AddChannel(Func<TChannel> channelFactory)
        {
            if (channelFactory == null)
                throw new ArgumentNullException(nameof(channelFactory));

            this.channelGroupFactory.AddChannelFactory(channelFactory);
        }

        public void ConfigurationOptions(Action<TOptions> optionsFactory)
        {
            if (optionsFactory == null)
                throw new ArgumentNullException(nameof(optionsFactory));

            this.optionsFactory = optionsFactory;
        }
        
        protected abstract TChannelGroup OnCreateChannelGroup(List<TChannel> channels);
        
        public virtual Task StopAsync()
        {
            this.optionsFactory = null;
            
            return Task.CompletedTask;
        }
    }
}