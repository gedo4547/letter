using System;
using System.Collections.Generic;

namespace Letter
{
    public class ChannelGroupFactoryDgramImpl<TContext, TChannel> : AChannelGroupFactory<ChannelGroupDgramImpl<TContext, TChannel>, TChannel, TContext>
        where TContext : IContext
        where TChannel : IDgramChannel<TContext>
    {
        public ChannelGroupFactoryDgramImpl(Func<List<TChannel>, ChannelGroupDgramImpl<TContext, TChannel>> channelGroupCreator) : base(channelGroupCreator)
        {
        }
    }
}