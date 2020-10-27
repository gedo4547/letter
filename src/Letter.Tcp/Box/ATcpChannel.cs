

using FilterGroupFactory = Letter.Bootstrap.ChannelFilterGroupFactory<Letter.Tcp.Box.ITcpSession, Letter.Tcp.Box.ITcpChannelFilter>;

namespace Letter.Tcp.Box
{
    abstract class ATcpChannel
    {
        public ATcpChannel(FilterGroupFactory groupFactory, SslFeature sslFeature)
        {
            this.sslFeature = sslFeature;
            this.groupFactory = groupFactory;
        }

        private SslFeature sslFeature;
        private FilterGroupFactory groupFactory;
        
        
        
        
    }
}