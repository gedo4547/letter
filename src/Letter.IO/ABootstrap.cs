using System;
using System.Threading.Tasks;

namespace Letter.IO
{ 
    public abstract class ABootstrap<TOptions, TSession, TChannel> : IBootstrap<TOptions, TSession, TChannel>
        where TOptions : class, IOptions, new()
        where TChannel : IChannel
        where TSession : ISession
    {
        protected TOptions options;
        
        private Action<TOptions> optionsFactory;
        private Action<IFilterPipeline<TSession>> filterPipelineHandler;
        
        public void ConfigurationOptions(Action<TOptions> optionsFactory)
        {
            if (optionsFactory == null)
            {
                throw new ArgumentNullException(nameof(optionsFactory));
            }
            
            this.optionsFactory = optionsFactory;
        }

        public void ConfigurationFilter(Action<IFilterPipeline<TSession>> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            this.filterPipelineHandler = handler;
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
            
            return this.ChannelFactoryAsync(this.options, this.filterPipelineHandler);
        }
        
        protected abstract Task<TChannel> ChannelFactoryAsync(TOptions options, Action<IFilterPipeline<TSession>> handler);
        
        public virtual ValueTask DisposeAsync()
        {
            return default;
        }
    }
}