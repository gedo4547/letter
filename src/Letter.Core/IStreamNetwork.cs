using System;

namespace Letter
{
    public interface IStreamNetwork<TSession, TChannel> : INetwork
        where TSession : ISession
        where TChannel : IStreamChannel<TSession>
    {
        void AddChannel(Func<TChannel> channelFactory);
    }
}