using System;
using System.Threading.Tasks;

namespace Letter.IO
{
    public abstract class AChannel<TSession, TOptions> : IChannel<TSession, TOptions> 
        where TSession : ISession
        where TOptions : IOptions
    {
        protected TOptions options;
        
        public void SettingOptions(TOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            
            this.options = options;
        }

        public void AddFilter(IFilter<TSession> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }
            
        }

        public abstract Task StopAsync();
    }
}