using System;
using System.Collections.Generic;
using Letter.IO;

namespace Letter
{
    public class ChannelGroupFactoryStreamImpl<TContext> : AChannelGroupFactory<ChannelGroupStreamImpl<TContext>, IStreamChannel<TContext>, TContext>
        where TContext : class, IContext
    {
        public ChannelGroupFactoryStreamImpl(Func<List<IStreamChannel<TContext>>, ChannelGroupStreamImpl<TContext>> channelGroupCreator) : base(channelGroupCreator)
        {
        }
    }
}