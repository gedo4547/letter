using System;
using System.Threading;
using System.Threading.Tasks;

namespace Letter
{
    public interface INetwork<TOptions> : IAsyncDisposable where TOptions : class, IOptions
    {
        void ConfigureOptions(Action<TOptions> optionsFactory);
        
        void Build();
        
        ValueTask CloseAsync(CancellationToken cancellationToken = default);
    }
}