﻿using System;
using System.IO;
using System.Net.Security;
using System.Threading.Tasks;

using FilterGroupFactory = Letter.StreamChannelFilterGroupFactory<Letter.Tcp.ITcpSession, Letter.Tcp.ITcpChannelFilter>;

namespace Letter.Tcp
{
    public abstract class ATcpBootstrap<TOptions, TChannel> : AStreamBootstrap<TOptions, ITcpSession, ITcpChannelFilter, TChannel>, ITcpBootstrap<TOptions, TChannel>
        where TOptions : ATcpOptions, new()
        where TChannel : IChannel
    {
        private SslFeature sslFeature;
        
        public void ConfigurationSsl(SslOptions sslOptions, Func<Stream, SslStream> sslStreamFactory)
        {
            if (sslOptions == null)
                throw new ArgumentNullException(nameof(sslOptions));
            if (sslStreamFactory == null)
                throw new ArgumentNullException(nameof(sslStreamFactory));

            this.sslFeature = new SslFeature()
            {
                sslOptions = sslOptions, 
                sslStreamFactory = sslStreamFactory
            };
        }
        
        protected override Task<TChannel> ChannelFactory(TOptions options, FilterGroupFactory groupFactory)
        {
            return this.ChannelFactory(options, groupFactory, this.sslFeature);
        }

        protected abstract Task<TChannel> ChannelFactory(TOptions options, FilterGroupFactory groupFactory, SslFeature sslFeature);
    }
    
    public class SslFeature
    {
        public SslOptions sslOptions;
        public Func<Stream, SslStream> sslStreamFactory;
    }
}