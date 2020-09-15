using System;

namespace Letter
{
    public interface IDgramNetwork<TSession, TChannel> : INetwork
        where TSession : ISession
        where TChannel : IDgramChannel<TSession>
    {
        void AddChannel(Func<TChannel> channelFactory);
    }
}