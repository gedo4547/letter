using System;
using Letter.Tcp;

namespace Letter
{
    public interface ITcpNetwork<TOptions> : IAsyncDisposable where TOptions : ATcpOptions
    {
        void ConfigureOptions(Action<TOptions> optionsFactory);
        
        void Build();
    }
}