using System;
using System.Threading.Tasks;

namespace Letter
{
    public interface IBootstrap<TOptions, TNetwork> : IAsyncDisposable
        where TOptions : class, IOptions, new()
        where TNetwork : INetwork
    {
        void ConfigurationNetwork(Action<TNetwork> configurator);
        
        void ConfigurationOptions(Action<TOptions> optionsFactory);
        
        Task<TNetwork> BuildAsync();
    }
}