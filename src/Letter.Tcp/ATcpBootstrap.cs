using System;
using System.IO;
using System.Net.Security;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    abstract class ATcpBootstrap<TOptions, TChannel> : ABootstrap<TOptions, ITcpSession, TChannel>, ITcpBootstrap<TOptions, TChannel>
        where TOptions : class, IOptions, new()
        where TChannel : IChannel
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

        protected override Task<TChannel> ChannelFactoryAsync(TOptions options, Action<IFilterPipeline<ITcpSession>> handler)
        {
            return this.ChannelFactoryAsync(options, handler, this.sslFeature);
        }
        
        protected abstract Task<TChannel> ChannelFactoryAsync(TOptions options, Action<IFilterPipeline<ITcpSession>> handler, SslFeature sslFeature);

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