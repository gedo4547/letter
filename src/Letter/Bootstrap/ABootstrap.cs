using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Letter
{
    public abstract class ABootstrap<TChannel, TSession, TOptions> : IBootstrap<TChannel, TSession, TOptions>
        where TSession : ISession
        where TOptions : IOptions
        where TChannel : IChannel<TSession>
    {
        protected ISocketsTrace trace;
        protected Action<TOptions> optionsFactory;
        protected List<Func<TChannel>> channelFactorys = new List<Func<TChannel>>();
        
        public void Logger(ISocketsTrace trace)
        {
            if (trace == null)
            {
                throw new ArgumentNullException(nameof(trace));
            }

            this.trace = trace;
        }

        public void Options(Action<TOptions> optionsFactory)
        {
            if (optionsFactory == null)
            {
                throw new ArgumentNullException(nameof(optionsFactory));
            }

            this.optionsFactory = optionsFactory;
        }

        public void Channel(Func<TChannel> channelFactory)
        {
            if (channelFactory == null)
            {
                throw new ArgumentNullException(nameof(channelFactory));
            }
            
            this.channelFactorys.Add(channelFactory);
        }

        protected List<TChannel> GetChannelList()
        {
            List<TChannel> channels = new List<TChannel>();
            for (int i = 0; i < this.channelFactorys.Count; i++)
            {
                var factory = this.channelFactorys[i];
                var channel = factory();
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
            if (this.trace != null)
            {
                this.trace = null;
            }

            if (this.optionsFactory != null)
            {
                this.optionsFactory = null;
            }

            if (this.channelFactorys != null)
            {
                this.channelFactorys.Clear();
                this.channelFactorys = null;
            }
            
            return Task.CompletedTask;
        }
    }
}