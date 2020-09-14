using System;
using System.Collections.Generic;

namespace Letter
{
    public class ChannelGroupFactoryStreamImpl<TContext, TChannel> : AChannelGroupFactory<ChannelGroupStreamImpl<TContext, TChannel>, TChannel, TContext>
        where TContext : IContext
        where TChannel : IStreamChannel<TContext>
    {
        public ChannelGroupFactoryStreamImpl(Func<List<TChannel>, ChannelGroupStreamImpl<TContext, TChannel>> channelGroupCreator) : base(channelGroupCreator)
        {
        }
    }
}