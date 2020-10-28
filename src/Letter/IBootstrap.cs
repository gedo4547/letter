using System;
using System.Threading.Tasks;

namespace Letter
{
    public interface IBootstrap<TOptions, TSession, TChannel, TChannelFilter> : IAsyncDisposable
        where TOptions : class, IOptions, new()
        where TChannel : IChannel
        where TSession : ISession
        where TChannelFilter : IChannelFilter<TSession>
    {
        void ConfigurationOptions(Action<TOptions> optionsFactory);
        
        Task<TChannel> BuildAsync();
    }
}