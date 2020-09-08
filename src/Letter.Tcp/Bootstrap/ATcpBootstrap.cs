using System.Net;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Tcp
{
    public abstract class ATcpBootstrap<TOptions> : ABootstrap<TOptions, ITcpChannel, ITcpContext, WrappedStreamReader, WrappedStreamWriter>, ITcpBootstrap<TOptions>
        where TOptions: ATcpOptions
    {
        protected BinaryOrder order;

        public abstract Task StartAsync(EndPoint point);

        protected void OnConnect(ITcpClient client)
        {
            var channels = base.CreateChannels();
            TcpContext context = new TcpContext(channels, this.order);
            
            context.Initialize(client);
        }
    }
}