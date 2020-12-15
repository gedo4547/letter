﻿using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;
using Letter.IO;
using Letter.Udp;

namespace Letter.Kcp
{
    public abstract class AKcpChannel<TOptions> : AChannel<IKcpSession, TOptions>, IFilter<IUdpSession>
        where TOptions : KcpOptions
    {
        public AKcpChannel(TOptions options, IUdpChannel channel, Action<IFilterPipeline<IKcpSession>> handler)
        {
            base.ConfigurationSelfOptions(options);
            base.ConfigurationSelfFilter(handler);
            
            this.channel = channel;
            this.channel.ConfigurationSelfFilter((pipeline) => { pipeline.Add(this); });
        }

        private IUdpChannel channel;
        private IUdpSession session;

        protected async Task InternalStartAsync(EndPoint address)
        {
            await this.channel.StartAsync(address);
        }

        public Task BindAsyns(EndPoint address)
        {
            return Task.CompletedTask;
        }

        public void OnTransportActive(IUdpSession session) => this.session = session;
        public void OnTransportInactive(IUdpSession session) => this.session = null;
        public void OnTransportException(IUdpSession session, Exception ex)
        {
            session.CloseAsync().NoAwait();
        }

        public void OnTransportRead(IUdpSession session, ref WrappedReader reader, WrappedArgs args)
        {
            ArraySegment<byte> message = (args.Value as List<ArraySegment<byte>>)[0];
            var flag = message.Array[message.Offset];
            
            if (flag == KcpProtocalType.SYN)
            {
                
            }
            else if (flag == KcpProtocalType.ACK)
            {
                
            }
            else if (flag == KcpProtocalType.FIN)
            {
                
            }
            else if (flag == KcpProtocalType.MSG)
            {
                
            }
        }

        public void OnTransportWrite(IUdpSession session, ref WrappedWriter writer, WrappedArgs args)
        {
            throw new NotImplementedException();
        }
    }
}