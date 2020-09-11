using System;
using System.Threading.Tasks;

namespace Letter
{
    public abstract class ANetwork<TOptions, TChannelGroupFactory, TChannelGroup, TChannel, TContext, TReader, TWriter> : INetwork<TOptions, TChannel, TContext, TReader, TWriter>
        where TOptions: IOptions
        where TReader : struct
        where TWriter : struct
        where TContext : class, IContext
        where TChannel : IChannel<TContext, TReader, TWriter>
        where TChannelGroup : AChannelGroup<TChannel, TContext, TReader, TWriter>
        where TChannelGroupFactory : AChannelGroupFactory<TChannelGroup, TChannel, TContext, TReader, TWriter>
    {
        public ANetwork(TOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.options = options;
        }

        protected TOptions options;
        private Action<TOptions> optionsFactory;
        protected abstract TChannelGroupFactory ChannelGroupFactory { get; }

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

        public void Build()
        {
            if (this.optionsFactory == null)
            {
                throw new NullReferenceException(nameof(this.optionsFactory));
            }

            this.optionsFactory(this.options);
        }

        public virtual Task CloseAsync()
        {
            return Task.CompletedTask;
        }

        public virtual ValueTask DisposeAsync()
        {
            this.optionsFactory = null;
            return default;
        }
    }
}