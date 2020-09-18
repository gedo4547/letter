using System;
using System.IO;
using System.Net.Security;

namespace Letter.Tcp
{
    public interface ITcpBootstrap<TOptions, TChannel> : IStreamBootstrap<TOptions, ITcpSession, ITcpChannelFilter, TChannel>
        where TOptions : ATcpOptions, new()
        where TChannel : IChannel
    {
        void ConfigurationSsl(SslOptions sslOptions, Func<Stream, SslStream> sslStreamFactory);
    }
}