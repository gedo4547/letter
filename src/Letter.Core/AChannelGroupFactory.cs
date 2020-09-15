using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Letter.Box.ssss
{
    public abstract class AChannelGroupFactory<TSession, TChannel, TChannelGroup> : IChannelGroupFactory<TSession, TChannel, TChannelGroup>
        where TSession : ISession
        where TChannel : IChannel<TSession>
        where TChannelGroup : IChannelGroup<TSession, TChannel>
    {
        private List<Func<TChannel>> channelFactorys = new List<Func<TChannel>>();
        
        public void AddChannelFactory(Func<TChannel> channelFactory)
        {
            if (channelFactory == null)
            {
                throw new  ArgumentNullException(nameof(channelFactory));
            }
            
            this.channelFactorys.Add(channelFactory);
        }

        public TChannelGroup CreateChannelGroup()
        {
            List<TChannel> channels = new List<TChannel>();
            foreach (var channelFactory in channelFactorys)
            {
                var channel = channelFactory();
                if (channel == null)
                    throw new NullReferenceException(nameof(channel));
                
                channels.Add(channel);
            }

            return this.ChannelGroupCreator(channels);
        }

        protected abstract TChannelGroup ChannelGroupCreator(List<TChannel> channels);
        

        public ValueTask DisposeAsync()
        {
            if (this.channelFactorys != null)
            {
                this.channelFactorys.Clear();
                this.channelFactorys = null;
            }

            return default;
        }
    }
}