﻿using System;
using System.Threading.Tasks;

namespace Letter.IO
{
    public abstract class AChannel<TSession, TOptions> : IChannel<TSession, TOptions> 
        where TSession : ISession
        where TOptions : IOptions
    {
        protected TOptions options;
        protected Action<IFilterPipeline<TSession>> handler;
        
        public void ConfigurationSelfOptions(TOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            this.options = options;
        }

        public void ConfigurationSelfFilter(Action<IFilterPipeline<TSession>> handler)
        {
            if (handler == null)
                return;

            this.handler += handler;
        }
        
        protected FilterPipeline<TSession> CreateFilterPipeline()
        {
            FilterPipeline<TSession> filterPipeline = new FilterPipeline<TSession>();
            if (this.handler != null)
            {
                this.handler(filterPipeline);
            }

            return filterPipeline;
        }

        public virtual Task StopAsync()
        {
            this.options = default;
            this.handler = null;

            return Task.CompletedTask;
        }
    }
}