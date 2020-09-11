using System;
using System.Threading.Tasks;

namespace Letter
{
    public abstract class ABootstrap<TOptions, TChannelGroupFactory, TChannelGroup, TChannel, TContext, TReader, TWriter> : IBootstrap<TOptions, TChannel, TContext, TReader, TWriter>
        where TOptions: IOptions
        where TReader : struct
        where TWriter : struct
        where TContext : class, IContext
        where TChannel : IChannel<TContext, TReader, TWriter>
        where TChannelGroup : AChannelGroup<TChannel, TContext, TReader, TWriter>
        where TChannelGroupFactory : AChannelGroupFactory<TChannelGroup, TChannel, TContext, TReader, TWriter>
    {
        protected Action<TOptions> optionsFactory;

        protected abstract TChannelGroupFactory ChannelGroupFactory
        {
            get;
        }
        
        public void AddChannel(Func<TChannel> channelFactory)
        {
            if (channelFactory == null)
                throw new ArgumentNullException(nameof(channelFactory));

            this.ChannelGroupFactory.AddChannelFactory(channelFactory);
        }

        public void ConfigurationOptions(Action<TOptions> optionsFactory)
        {
            if (optionsFactory == null)
                throw new ArgumentNullException(nameof(optionsFactory));

            this.optionsFactory = optionsFactory;
        }

        public virtual Task StopAsync()
        {
            this.optionsFactory = null;
            
            return Task.CompletedTask;
        }
    }
}