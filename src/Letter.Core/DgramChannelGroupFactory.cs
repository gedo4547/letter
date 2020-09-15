using System.Collections.Generic;

namespace Letter.Box.ssss
{
    public class DgramChannelGroupFactory<TSession, TChannel> : AChannelGroupFactory<TSession, TChannel, DgramChannelGroup<TSession, TChannel>>, IChannelGroupFactory<TSession, TChannel, DgramChannelGroup<TSession, TChannel>>
        where TSession : ISession
        where TChannel : IDgramChannel<TSession>
    {
        protected override DgramChannelGroup<TSession, TChannel> ChannelGroupCreator(List<TChannel> channels)
        {
            return new DgramChannelGroup<TSession, TChannel>(channels);
        }
    }
}