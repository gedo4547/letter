using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Tcp
{
    public abstract class ATcpBootstrap<TOptions> : ABootstrap<TOptions, TcpChannelGroup, ITcpChannel, ITcpContext, WrappedStreamReader, WrappedStreamWriter>, ITcpBootstrap<TOptions>
        where TOptions: ATcpOptions
    {
        protected BinaryOrder order;

        public abstract Task StartAsync(EndPoint point);

        protected override TcpChannelGroup OnCreateChannelGroup(List<ITcpChannel> channels)
        {
            return new TcpChannelGroup(channels);
        }

        protected void OnConnect(ITcpClient client)
        {
            TcpContext context = new TcpContext(this.channelGroupFactory.CreateChannelGroup(), this.order);
            
            context.Initialize(client);
        }
    }
}