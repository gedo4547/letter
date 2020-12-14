using System;
using System.Threading.Tasks;

namespace Letter.IO
{
    public interface IBootstrap<TOptions, TSession, TChannel> : IAsyncDisposable
        where TOptions : class, IOptions, new()
        where TChannel : IChannel<TSession, TOptions>
        where TSession : ISession
    {
        void ConfigurationGlobalOptions(Action<TOptions> handler);
        void ConfigurationGlobalFilter(Action<IFilterPipeline<TSession>> handler);

        Task BuildAsync();

        Task<TChannel> CreateAsync();
    }
}