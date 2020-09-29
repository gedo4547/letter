using System;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    abstract class ATcpNetwork<TTcpOptions> : ITcpNetwork<TTcpOptions>
        where TTcpOptions : ATcpOptions
    {
        public ATcpNetwork(TTcpOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.options = options;
        }
        
        protected TTcpOptions options;
        private Action<TTcpOptions> optionsFactory;
        
        public void ConfigureOptions(Action<TTcpOptions> optionsFactory)
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
            
            try
            {
                 this.optionsFactory(this.options);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"当前的线程：{System.Threading.Thread.CurrentThread.ManagedThreadId},异常：{ex.ToString()}");
                throw ex;
            }
        }
        
        public virtual ValueTask DisposeAsync()
        {
            this.options = null;
            this.optionsFactory = null;

            return default;
        }
        
        
    }
}