using System;
using System.Threading.Tasks;

namespace Letter
{
    public interface IBootstrap<TOptions, TChannel, TContext> : IAsyncDisposable
        where TOptions : IOptions
        where TContext : IContext
        where TChannel : IChannel<TContext>
    {
        void AddChannel(Func<TChannel> channelFactory);

        void ConfigurationOptions(Action<TOptions> optionsFactory);

        void Build();

        Task CloseAsync();
    }
}