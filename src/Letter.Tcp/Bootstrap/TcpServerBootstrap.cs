using System.Threading.Tasks;

using FilterGroupFactory = Letter.StreamChannelFilterGroupFactory<Letter.Tcp.ITcpSession, Letter.Tcp.ITcpChannelFilter>;

namespace Letter.Tcp
{
    public class TcpServerBootstrap :  ATcpBootstrap<TcpServerOptions, ITcpServerChannel>, ITcpServerBootstrap
    {
        protected override Task<ITcpServerChannel> ChannelFactory(TcpServerOptions options, FilterGroupFactory groupFactory)
        {
            throw new System.NotImplementedException();
        }
    }
}