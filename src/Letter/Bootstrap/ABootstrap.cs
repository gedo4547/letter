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
        
        protected Action<TOptions> optionsFactory;
        protected List<Func<TChannel>> channelFactorys = new List<Func<TChannel>>();
        
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

        protected List<TChannel> CreateChannels()
        {
            List<TChannel> channels = new List<TChannel>();
            for (int i = 0; i < this.channelFactorys.Count; i++)
            {
                var channel = this.channelFactorys[i]();
                if (channel == null)
                {
                    throw new NullReferenceException("The value returned by channelFactorys is null");
                }
                
                channels.Add(channel);
            }

            return channels;
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