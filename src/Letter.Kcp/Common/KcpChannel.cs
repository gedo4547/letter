using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;
using Letter.IO;
using Letter.Udp;

namespace Letter.Kcp
{
    sealed class KcpChannel : AChannel<IKcpSession, KcpOptions>, IKcpChannel, IFilter<IUdpSession>
    {
        public KcpChannel(KcpOptions options, IUdpChannel channel, IKcpThread thread, Action<IFilterPipeline<IKcpSession>> handler)
        {
            base.ConfigurationSelfOptions(options);
            base.ConfigurationSelfFilter(handler);

            this.thread = thread;
            this.channel = channel;
            this.channel.ConfigurationSelfFilter((pipeline) => { pipeline.Add(this); });
        }

        private IUdpChannel channel;
        private IUdpSession session;
        
        private IKcpThread thread;
        private IChannelRouter router;
        private Dictionary<uint, KcpSession> sessions = new Dictionary<uint, KcpSession>();
        
        public void ConfigurationRouter(IChannelRouter router)
        {
            if (router == null)
            {
                throw new ArgumentNullException(nameof(router));
            }

            this.router = router;
        }
        
        public async Task BindAsync(EndPoint address)
        {
            await this.channel.StartAsync(address);
        }
        
        public bool Connect(uint conv, EndPoint remoteAddress)
        {
            if (this.sessions.ContainsKey(conv))
            {
                return false;
            }
            
            var localAddress = this.session.LocalAddress;
            FilterPipeline<IKcpSession> pipeline = base.CreateFilterPipeline();
            var kcpSession = new KcpSession(conv, remoteAddress, localAddress, options, session, thread, pipeline);
            this.sessions.Add(conv, kcpSession);

            return true;
        }

        public void OnTransportActive(IUdpSession session) => this.session = session;
        public void OnTransportInactive(IUdpSession session) => this.session = null;

        public void OnTransportException(IUdpSession session, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void OnTransportRead(IUdpSession session, ref WrappedReader reader, WrappedArgs args)
        {
            var conv = reader.ReadUInt32();
            if (!this.sessions.ContainsKey(conv))
            {
                return;
            }

            var kcpSession = this.sessions[conv];
            if (kcpSession.RemoteAddress != session.RcvAddress)
            {
                return;
            }

            ReadOnlySequence<byte> buffer = reader.ReadBuffer((int)reader.Length);
            kcpSession.ReceiveMessage(ref buffer);
        }

        public void OnTransportWrite(IUdpSession session, ref WrappedWriter writer, WrappedArgs args)
        {
            throw new NotImplementedException();
        }
    }
}