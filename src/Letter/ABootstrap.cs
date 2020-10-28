﻿using System;
using System.Threading.Tasks;

namespace Letter
{
    public abstract class ABootstrap<TOptions, TSession, TChannel, TChannelFilter> : IBootstrap<TOptions, TSession, TChannel, TChannelFilter>
        where TOptions : class, IOptions, new()
        where TChannel : IChannel
        where TSession : ISession
        where TChannelFilter : IChannelFilter<TSession>
    {
        public ABootstrap()
        {
            this.filterGroupFactory = new ChannelFilterGroupFactory<TSession, TChannelFilter>();
        }

        protected TOptions options;
        
        private Action<TOptions> optionsFactory;
        private ChannelFilterGroupFactory<TSession, TChannelFilter> filterGroupFactory;
        
        public void ConfigurationOptions(Action<TOptions> optionsFactory)
        {
            if (optionsFactory == null)
            {
                throw new ArgumentNullException(nameof(optionsFactory));
            }
            
            this.optionsFactory = optionsFactory;
        }

        public virtual Task<TChannel> BuildAsync()
        {
            if (this.options == null)
            {
                if (this.optionsFactory == null)
                {
                    throw new NullReferenceException(nameof(this.optionsFactory));
                }
                this.options = new TOptions();
                this.optionsFactory(this.options);
            }

            return this.ChannelFactory(this.options, this.filterGroupFactory);
        }
        
        public void AddChannelFilter<TFilter>() where TFilter : TChannelFilter, new()
        {
            this.AddChannelFilter(() => { return new TFilter(); });
        }

        public void AddChannelFilter(Func<TChannelFilter> func)
        {
           this.filterGroupFactory.AddFilterCreator(func);
        }
        
        protected abstract Task<TChannel> ChannelFactory(TOptions options, ChannelFilterGroupFactory<TSession, TChannelFilter> filterGroupFactory);

        public virtual async ValueTask DisposeAsync()
        {
            await this.filterGroupFactory.DisposeAsync();
        }
    }
}