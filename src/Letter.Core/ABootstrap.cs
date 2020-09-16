using System;
using System.Threading.Tasks;

namespace Letter
{
    public abstract class ABootstrap<TOptions, TChannel> : IBootstrap<TOptions, TChannel>
        where TOptions : class, IOptions, new()
        where TChannel : IChannel
    {
        protected TOptions options;
        
        private Action<TOptions> optionsFactory;
        
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
                
                this.optionsFactory(this.options);
            }

            return null;
        }

        
        public virtual ValueTask DisposeAsync()
        {
            this.optionsFactory = null;
            this.options = default;
            
            return default;
        }
    }
}