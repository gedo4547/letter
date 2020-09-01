using System;
using System.Threading.Tasks;

namespace Letter
{
    public abstract class ANetwork<TOptions> : INetwork<TOptions> where TOptions : class, IOptions
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
        protected ISocketsTrace trace; 
        
        private Action<TOptions> optionsFactory;
        

        public void ConfigureOptions(Action<TOptions> optionsFactory)
        {
            if (optionsFactory == null)
            {
                throw new ArgumentNullException(nameof(optionsFactory));
            }

            this.optionsFactory = optionsFactory;
        }

        public virtual void Build()
        {
            if(this.optionsFactory == null)
            {
                throw new ArgumentNullException(nameof(optionsFactory));
            }
            
            this.optionsFactory(this.options);
        }

        public virtual Task StopAsync()
        {
            this.options = null;
            this.optionsFactory = null;
            
            return Task.CompletedTask;
        }
    }
}