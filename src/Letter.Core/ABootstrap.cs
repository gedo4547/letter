using System;
using System.Threading.Tasks;

namespace Letter.Box.ssss
{
    public abstract class ABootstrap<TOptions, TNetwork> : IBootstrap<TOptions, TNetwork>
        where TOptions : IOptions, new()
        where TNetwork : INetwork
    {
        protected TOptions options = new TOptions();
        
        private Action<TOptions> optionsFactory;
        
        public void ConfigurationOptions(Action<TOptions> optionsFactory)
        {
            if (optionsFactory == null)
            {
                throw new ArgumentNullException(nameof(optionsFactory));
            }
            
            this.optionsFactory = optionsFactory;
        }

        public virtual Task<TNetwork> BuildAsync()
        {
            if (this.optionsFactory != null)
            {
                throw new NullReferenceException(nameof(this.optionsFactory));
            }

            this.optionsFactory(this.options);

            return this.NetworkCreator();
        }

        protected abstract Task<TNetwork> NetworkCreator();
        
        public virtual ValueTask DisposeAsync()
        {
            this.optionsFactory = null;
            this.options = default;

            return default;
        }
    }
}