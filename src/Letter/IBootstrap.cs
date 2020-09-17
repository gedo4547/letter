using System;
using System.Threading.Tasks;

namespace Letter
{
    public interface IBootstrap<TOptions, TChannel> : IAsyncDisposable
        where TOptions : class, IOptions, new()
        where TChannel : IChannel
    {
        void ConfigurationOptions(Action<TOptions> optionsFactory);
        
        Task<TChannel> BuildAsync();
    }
}