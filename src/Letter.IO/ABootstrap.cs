﻿using System;
using System.Threading.Tasks;

namespace Letter.IO
{ 
    public abstract class ABootstrap<TOptions, TSession, TChannel> : IBootstrap<TOptions, TSession, TChannel>
        where TOptions : class, IOptions, new()
        where TChannel : IChannel<TSession, TOptions>
        where TSession : ISession
    {
        protected TOptions options = new TOptions();
        
        private Action<TOptions> optionsFactory;
        private Action<IFilterPipeline<TSession>> filterPipelineHandler;
        
        public void ConfigurationGlobalOptions(Action<TOptions> optionsFactory)
        {
            if (optionsFactory == null)
            {
                throw new ArgumentNullException(nameof(optionsFactory));
            }
            
            this.optionsFactory = optionsFactory;
        }

        public void ConfigurationGlobalFilter(Action<IFilterPipeline<TSession>> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            this.filterPipelineHandler = handler;
        }
        
        public virtual Task BuildAsync()
        {
            if (this.optionsFactory != null)
            {
                this.optionsFactory(this.options);
            }
            
            return Task.CompletedTask;
        }

        public Task<TChannel> CreateChannelAsync()
        {
            return this.ChannelFactoryAsync(this.options, this.filterPipelineHandler);
        }

        protected abstract Task<TChannel> ChannelFactoryAsync(TOptions options, Action<IFilterPipeline<TSession>> handler);
        
        public virtual ValueTask DisposeAsync()
        {
            return default;
        }
    }
}