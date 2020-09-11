using System;
using System.Collections.Generic;
using Letter.IO;

namespace Letter
{
    public class ChannelGroupFactoryDgramImpl<TContext> : AChannelGroupFactory<ChannelGroupDgramImpl<TContext>, IDgramChannel<TContext>, TContext, WrappedDgramReader, WrappedDgramWriter >
        where TContext : class, IContext
    {
        public ChannelGroupFactoryDgramImpl(Func<List<IDgramChannel<TContext>>, ChannelGroupDgramImpl<TContext>> channelGroupCreator) : base(channelGroupCreator)
        {
        }
    }
}