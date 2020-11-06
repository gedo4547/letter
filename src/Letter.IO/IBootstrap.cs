using System;
using System.Threading.Tasks;

namespace Letter.IO
{
    public interface IBootstrap<TOptions, TSession, TChannel> : IAsyncDisposable
        where TOptions : class, IOptions, new()
        where TChannel : IChannel
        where TSession : ISession
    {
        void ConfigurationOptions(Action<TOptions> handler);
        void ConfigurationFilter(Action<IFilterPipeline<TSession>> handler);

        Task<TChannel> BuildAsync();
    }
}