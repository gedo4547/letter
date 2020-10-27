using System;
using System.IO;
using System.Net.Security;

namespace Letter.Tcp.Box
{
    public interface ITcpBootstrap<TOptions, TChannel> : Letter.Bootstrap.IBootstrap<TOptions, ITcpSession, TChannel, ITcpChannelFilter>
        where TOptions : class, Bootstrap.IOptions, new()
        where TChannel : Bootstrap.IChannel
    {
        void ConfigurationSsl(SslOptions sslOptions, Func<Stream, SslStream> sslStreamFactory);
    }
}