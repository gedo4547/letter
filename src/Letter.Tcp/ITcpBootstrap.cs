using System;
using System.IO;
using System.Net.Security;
using Letter.IO;

namespace Letter.Tcp
{
    public interface ITcpBootstrap<TOptions, TChannel> : IBootstrap<TOptions, ITcpSession, TChannel>
        where TOptions : class, IOptions, new()
        where TChannel : IChannel
    {
        void ConfigurationSsl(SslOptions sslOptions, Func<Stream, SslStream> sslStreamFactory);
    }
}