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
        private IChannelController controller;
        
        public void ConfigurationController(IChannelController controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            this.controller = controller;
        }
        
        public async Task BindAsync(EndPoint address)
        {
            await this.channel.StartAsync(address);
        }
        
        public bool Connect(uint conv, EndPoint remoteAddress)
        {
            var localAddress = this.session.LocalAddress;
            var pipeline = base.CreateFilterPipeline();
            return this.controller.RegisterSession(new KcpSession(conv, remoteAddress, localAddress, options, session, thread, pipeline));
        }

        public void OnTransportActive(IUdpSession session) => this.session = session;
        public void OnTransportInactive(IUdpSession session) => this.session = null;

        public void OnTransportException(IUdpSession session, Exception ex)
        {
            session.CloseAsync().NoAwait();
        }

        public void OnTransportRead(IUdpSession session, ref WrappedReader reader, WrappedArgs args)
        {
            this.controller.OnRcvUdpMessage(session, ref reader, args);
        }

        public void OnTransportWrite(IUdpSession session, ref WrappedWriter writer, WrappedArgs args)
        {
            this.controller.OnSndUdpMessage(session, ref writer, args);
        }

        public override Task StopAsync()
        {
            this.controller.Dispose();
            return base.StopAsync();
        }
    }
}