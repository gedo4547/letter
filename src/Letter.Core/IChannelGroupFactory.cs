using System;

namespace Letter.Box.ssss
{
    public interface IChannelGroupFactory<TSession, TChannel, TChannelGroup> : IAsyncDisposable
        where TSession : ISession
        where TChannel : IChannel<TSession>
        where TChannelGroup : IChannelGroup<TSession, TChannel>
    {
        void AddChannelFactory(Func<TChannel> channelFactory);

        TChannelGroup CreateChannelGroup();
    }
}