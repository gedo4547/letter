﻿using System;
using System.Threading.Tasks;

namespace Letter
{
    public abstract class ABootstrap<TOptions, TChannelGroupFactory, TChannelGroup, TChannel, TContext> : IBootstrap<TOptions, TChannel, TContext>
        where TOptions : IOptions
        where TContext : IContext
        where TChannel : IChannel<TContext>
        where TChannelGroup : AChannelGroup<TChannel, TContext>
        where TChannelGroupFactory : AChannelGroupFactory<TChannelGroup, TChannel, TContext>
    {
        public ABootstrap(TOptions options)
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

        public virtual void Build()
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