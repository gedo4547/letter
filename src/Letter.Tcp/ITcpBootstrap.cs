using System;
using System.IO;
using System.Net.Security;

namespace Letter.Tcp
{
    public interface ITcpBootstrap<TOptions, TChannel> : Letter.IBootstrap<TOptions, ITcpSession, TChannel, ITcpChannelFilter>
        where TOptions : class, IOptions, new()
        where TChannel : IChannel
    {
        void ConfigurationSsl(SslOptions sslOptions, Func<Stream, SslStream> sslStreamFactory);
    }
}