using System.Net;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Tcp
{
    public interface ITcpBootstrap<TOptions> : IBootstrap<TOptions, ITcpChannel, ITcpContext, WrappedStreamReader, WrappedStreamWriter>
        where TOptions : ATcpOptions
    {
        Task StartAsync(EndPoint point);
    }
}