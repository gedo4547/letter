using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Letter.Box
{
    public abstract class ABootstrap<TTransport, TContext, TSession, TOptions, TChannel, TReader, TWriter>
        : IBootstrap<TTransport, TContext, TSession, TOptions, TChannel, TReader, TWriter>
        where TContext : class, IContext
        where TReader : struct
        where TWriter : struct
        where TOptions : IOptions
        where TSession : ISession
        where TChannel : IChannel<TContext, TReader, TWriter>
        where TTransport : ITransport<TSession, TChannel, TContext, TReader, TWriter>
    {
        private Action<TOptions> optionsFactory;
        private Func<TTransport> transportFactory;
        private List<Func<TChannel>> channelFactorys = new List<Func<TChannel>>();
        
        public void ConfigureOptions(Action<TOptions> optionsFactory)
        {
            if (optionsFactory == null)
            {
                throw new ArgumentNullException(nameof(optionsFactory));
            }

            this.optionsFactory = optionsFactory;
        }

        public void ConfigureTransport(Func<TTransport> transportFactory)
        {
            if (transportFactory == null)
            {
                throw new ArgumentNullException(nameof(transportFactory));
            }

            this.transportFactory = transportFactory;
        }

        public void ConfigureChannel(Func<TChannel> channelFactory)
        {
            if (channelFactory == null)
            {
                throw new ArgumentNullException(nameof(channelFactory));
            }
            
            this.channelFactorys.Add(channelFactory);
        }

        protected List<TChannel> GetChannels()
        {
            List<TChannel> channels = new List<TChannel>();
            for (int i = 0; i < this.channelFactorys.Count; i++)
            {
                var channelFactory = this.channelFactorys[i];
                var channel = channelFactory();
                if (channel == null)
                {
                    throw new NullReferenceException("channel is null");
                }

                channels.Add(channel);
            }
            return channels;
        }
        
        public abstract Task StartAsync(EndPoint point);

        public virtual Task StopAsync()
        {
            this.optionsFactory = null;
            this.transportFactory = null;
            
            if (this.channelFactorys != null)
            {
                this.channelFactorys.Clear();
                this.channelFactorys = null;
            }

            return Task.CompletedTask;
        }
    }
}