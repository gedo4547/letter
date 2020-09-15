using System;
using System.Threading.Tasks;

namespace Letter
{
    public abstract class ABootstrap<TOptions, TNetwork> : IBootstrap<TOptions, TNetwork>
        where TOptions : class, IOptions, new()
        where TNetwork : INetwork
    {
        protected TOptions options;
        
        private Action<TOptions> optionsFactory;
        private Action<TNetwork> networkConfigurator;
        
        public void ConfigurationNetwork(Action<TNetwork> configurator)
        {
            if (configurator == null)
            {
                throw new ArgumentNullException(nameof(configurator));
            }

            this.networkConfigurator = configurator;
        }

        public void ConfigurationOptions(Action<TOptions> optionsFactory)
        {
            if (optionsFactory == null)
            {
                throw new ArgumentNullException(nameof(optionsFactory));
            }
            
            this.optionsFactory = optionsFactory;
        }

        public virtual async Task<TNetwork> BuildAsync()
        {
            if (this.options == null)
            {
                if (this.optionsFactory != null)
                {
                    throw new NullReferenceException(nameof(this.optionsFactory));
                }
                
                this.optionsFactory(this.options);
            }
            
            TNetwork network = await this.NetworkCreator();
            this.networkConfigurator?.Invoke(network);
            
            return network;
        }

        protected abstract Task<TNetwork> NetworkCreator();
        
        public virtual ValueTask DisposeAsync()
        {
            this.optionsFactory = null;
            this.options = default;
            this.networkConfigurator = null;
            
            return default;
        }
    }
}