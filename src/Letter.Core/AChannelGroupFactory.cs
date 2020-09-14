using System;
using System.Collections.Generic;

namespace Letter
{
    public abstract class AChannelGroupFactory<TChannelGroup, TChannel, TContext> : IDisposable
        where TContext : IContext
        where TChannel : IChannel<TContext>
        where TChannelGroup : AChannelGroup<TChannel, TContext>
    {
        public AChannelGroupFactory(Func<List<TChannel>, TChannelGroup> channelGroupCreator)
        {
            if (channelGroupCreator == null)
            {
                throw new ArgumentNullException(nameof(channelGroupCreator));
            }

            this.channelGroupCreator = channelGroupCreator;
        }
        
        private Func<List<TChannel>, TChannelGroup> channelGroupCreator;
        private List<Func<TChannel>> channelFactorys = new List<Func<TChannel>>();

        public void AddChannelFactory(Func<TChannel> channelFactory)
        {
            if (channelFactory == null)
            {
                throw new ArgumentNullException(nameof(channelFactory));
            }
            
            this.channelFactorys.Add(channelFactory);
        }

        public TChannelGroup CreateChannelGroup()
        {
            List<TChannel> channels = new List<TChannel>();
            int count = this.channelFactorys.Count;
            for (int i = 0; i < count; i++)
            {
                var channel = this.channelFactorys[i]();
                if (channel == null)
                {
                    throw new NullReferenceException(nameof(channel));
                }
                
                channels.Add(channel);
            }

            return this.channelGroupCreator(channels);
        }

        public void Dispose()
        {
            this.channelGroupCreator = null;
            
            if (this.channelFactorys != null)
            {
                this.channelFactorys.Clear();
                this.channelFactorys = null;
            }
        }
    }
}