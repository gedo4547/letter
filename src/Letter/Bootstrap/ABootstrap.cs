using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Letter
{
    public abstract class ABootstrap<TOptions, TChannel, TContext, TReader, TWriter> : IBootstrap<TOptions, TChannel, TContext, TReader, TWriter>
        where TOptions: IOptions
        where TReader : struct
        where TWriter : struct
        where TContext : class, IContext
        where TChannel : IChannel<TContext, TReader, TWriter>
    {
        protected List<Func<TChannel>> channelFactorys;
        protected Action<TOptions> optionsFactory;
        
        
        public void AddChannel(Func<TChannel> channelFactory)
        {
            if (channelFactory == null)
            {
                throw new ArgumentNullException(nameof(channelFactory));
            }

            this.channelFactorys.Add(channelFactory);
        }

        public void ConfigurationOptions(Action<TOptions> optionsFactory)
        {
            if (optionsFactory == null)
            {
                throw new ArgumentNullException(nameof(optionsFactory));
            }

            this.optionsFactory = optionsFactory;
        }
        
        public virtual Task StopAsync()
        {
            if (this.channelFactorys != null)
            {
                this.channelFactorys.Clear();
                this.channelFactorys = null;
            }
            this.optionsFactory = null;
            
            return Task.CompletedTask;
        }
    }
}