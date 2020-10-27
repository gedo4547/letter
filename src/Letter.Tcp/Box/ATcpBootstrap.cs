using System;
using System.IO;
using System.Net.Security;
using System.Threading.Tasks;
using Letter.Bootstrap;

using FilterGroupFactory = Letter.Bootstrap.ChannelFilterGroupFactory<Letter.Tcp.Box.ITcpSession, Letter.Tcp.Box.ITcpChannelFilter>;

namespace Letter.Tcp.Box
{
    abstract class ATcpBootstrap<TOptions, TChannel> : ABootstrap<TOptions, ITcpSession, TChannel, ITcpChannelFilter>, ITcpBootstrap<TOptions, TChannel>
        where TOptions : class, Bootstrap.IOptions, new()
        where TChannel : Bootstrap.IChannel
    {
        private SslFeature sslFeature;
        
        public void ConfigurationSsl(SslOptions sslOptions, Func<Stream, SslStream> sslStreamFactory)
        {
            if (sslOptions == null)
                throw new ArgumentNullException(nameof(sslOptions));
            if (sslStreamFactory == null)
                throw new ArgumentNullException(nameof(sslStreamFactory));
            
            this.sslFeature = new SslFeature(sslOptions, sslStreamFactory);
        }
        
        protected override Task<TChannel> ChannelFactory(TOptions options, FilterGroupFactory groupFactory)
        {
            return this.ChannelFactory(options, groupFactory, this.sslFeature);
        }

        protected abstract Task<TChannel> ChannelFactory(TOptions options, FilterGroupFactory groupFactory, SslFeature sslFeature);

        public override ValueTask DisposeAsync()
        {
            return base.DisposeAsync();
        }
    }
    
    class SslFeature
    {
        public SslFeature(SslOptions sslOptions, Func<Stream, SslStream> sslStreamFactory)
        {
            this.sslOptions = sslOptions;
            this.sslStreamFactory = sslStreamFactory;
        }

        public readonly SslOptions sslOptions;
        public readonly Func<Stream, SslStream> sslStreamFactory;
    }
}