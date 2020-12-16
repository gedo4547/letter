using System;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;
using Letter.IO;
using Letter.Udp;

namespace Letter.Kcp
{
    sealed class KcpChannel : AChannel<IKcpSession, KcpOptions>, IKcpChannel, IFilter<IUdpSession>
    {
        public KcpChannel(KcpOptions options, IUdpChannel channel, IKcpScheduler scheduler, Action<IFilterPipeline<IKcpSession>> handler)
        {
            base.ConfigurationSelfOptions(options);
            base.ConfigurationSelfFilter(handler);

            this.scheduler = scheduler;
            this.channel = channel;
            this.channel.ConfigurationSelfFilter((pipeline) => { pipeline.Add(this); });
        }

        private IUdpChannel channel;
        private IUdpSession session;
        
        private IKcpScheduler scheduler;
        
        public async Task BindAsync(EndPoint address)
        {
            await this.channel.StartAsync(address);
        }
        
        public IKcpSession CreateSession(EndPoint remoteAddress)
        {
            FilterPipeline<IKcpSession> pipeline = base.CreateFilterPipeline();
            var kcpSession = new KcpSession(remoteAddress, session.LocalAddress, options, session, scheduler, pipeline);
            
            return kcpSession;
        }

        public void OnTransportActive(IUdpSession session) => this.session = session;
        public void OnTransportInactive(IUdpSession session) => this.session = null;

        public void OnTransportException(IUdpSession session, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void OnTransportRead(IUdpSession session, ref WrappedReader reader, WrappedArgs args)
        {
            throw new NotImplementedException();
        }

        public void OnTransportWrite(IUdpSession session, ref WrappedWriter writer, WrappedArgs args)
        {
            throw new NotImplementedException();
        }
    }
}