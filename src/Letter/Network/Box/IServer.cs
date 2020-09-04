using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Letter.Box
{
    public interface IServer<TServerOptions, TClientOptions, TClient> : INetwork<TServerOptions>
        where TServerOptions : class, IOptions
        where TClientOptions : class, IOptions
        where TClient : IClient<TClientOptions>
    {
        EndPoint BindAddress { get; }

        void Bind(EndPoint point);

        ValueTask<TClient> AcceptAsync(CancellationToken cancellationToken = default);
    }
}