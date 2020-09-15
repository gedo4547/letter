using System.Collections.Generic;

namespace Letter.Box.ssss
{
    public class StreamChannelGroupFactory<TSession, TChannel> : AChannelGroupFactory<TSession, TChannel, StreamChannelGroup<TSession, TChannel>>, IChannelGroupFactory<TSession,TChannel, StreamChannelGroup<TSession, TChannel>>
        where TSession : ISession
        where TChannel : IStreamChannel<TSession>
    {
        protected override StreamChannelGroup<TSession, TChannel> ChannelGroupCreator(List<TChannel> channels)
        {
            return new StreamChannelGroup<TSession, TChannel>(channels);
        }
    }
}