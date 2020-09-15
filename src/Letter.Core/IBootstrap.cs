using System;
using System.Threading.Tasks;

namespace Letter.Box.ssss
{
    public interface IBootstrap<TOptions, TNetwork> : IAsyncDisposable
        where TOptions : IOptions, new()
        where TNetwork : INetwork
    {
        void ConfigurationOptions(Action<TOptions> optionsFactory);
        
        Task<TNetwork> BuildAsync();
    }
}