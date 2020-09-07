using System.Net;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Tcp
{
    public abstract class ATcpBootstrap<TOptions> : ABootstrap<TOptions, ITcpChannel, ITcpContext, WrappedStreamReader, WrappedStreamWriter>, ITcpBootstrap<TOptions>
        where TOptions: IOptions
    {
        public abstract Task StartAsync(EndPoint point);

        protected void OnConnect(ITcpClient client)
        {
            TcpContext context = new TcpContext();
            context.Initialize(client);
        }
    }
}