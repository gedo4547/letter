using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;

using Letter.IO;
using Letter.Udp;

namespace Letter.Kcp
{
    sealed class KcpChannel : AChannel<IKcpSession, KcpOptions>, IKcpChannel, IFilter<IUdpSession>, IKcpSessionCreator
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
        
        private bool isStop = false;
        private bool isInvalid = false;
        private AKcpController controller;

        public bool IsActivate => !isInvalid;

        public void ConfigurationSelfController(AKcpController controller)
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

        public IKcpSession Create(uint conv, EndPoint remoteAddress, IKcpClosable closable)
        {
            if (this.isStop)
                throw new Exception("The channel has stopped working");
            if (this.isInvalid)
                throw new Exception("Channel failure, call the StopAsync method to release the channel");

            var localAddress = this.session.LocalAddress;
            var pipeline = base.CreateFilterPipeline();
            return new KcpSession(conv, remoteAddress, localAddress, options, session, thread, pipeline, closable);
        }

        public void OnTransportActive(IUdpSession session) => this.session = session;
        public void OnTransportInactive(IUdpSession session) => this.session = null;

        public void OnTransportException(IUdpSession session, Exception ex)
        {
            this.isInvalid = true;
            this.controller.OnUdpException(session, ex);
        }

        public void OnTransportRead(IUdpSession session, ref WrappedReader reader, WrappedArgs args)
        {
            if(this.isInvalid || this.isStop) return;

            this.controller.OnUdpInput(session, ref reader, args);
        }

        public void OnTransportWrite(IUdpSession session, ref WrappedWriter writer, WrappedArgs args)
        {
            if(this.isInvalid || this.isStop) return;

            this.controller.OnUdpOutput(session, ref writer, args);
        }

        public override async Task StopAsync()
        {
            this.isStop = true;

            await base.StopAsync();

            if(this.channel != null)
            {
                await this.channel.StopAsync();
                this.channel = null;
            }

            this.session = null;
            this.controller.Dispose();
        }
    }
}