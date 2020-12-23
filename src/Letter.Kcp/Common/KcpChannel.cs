using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;
using Letter.IO;
using Letter.Udp;

using Kcptun = System.Net.Sockets.Kcp.Kcp;

namespace Letter.Kcp
{
    sealed class KcpChannel : AChannel<IKcpSession, KcpOptions>, IKcpChannel, IFilter<IUdpSession>
    {
        public KcpChannel(KcpOptions options, IUdpChannel channel, IKcpThread thread, Action<IFilterPipeline<IKcpSession>> handler)
        {
            base.ConfigurationSelfOptions(options);
            base.ConfigurationSelfFilter(handler);
            
            var order = Kcptun.IsLittleEndian ? BinaryOrder.LittleEndian : BinaryOrder.BigEndian;
            this.binaryOrderOperators = BinaryOrderOperatorsFactory.GetOperators(order);

            this.thread = thread;
            this.channel = channel;
            this.channel.ConfigurationSelfFilter((pipeline) => { pipeline.Add(this); });
        }

        private IUdpChannel channel;
        private IUdpSession session;
        
        private IKcpThread thread;
        private IBinaryOrderOperators binaryOrderOperators;
        
        private IChannelController controller;
        private Dictionary<uint, KcpSession> sessions = new Dictionary<uint, KcpSession>();
        
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
            if (this.sessions.ContainsKey(conv))
            {
                return false;
            }
            
            var localAddress = this.session.LocalAddress;
            var pipeline = base.CreateFilterPipeline();
            var kcpSession = new KcpSession(conv, remoteAddress, localAddress, options, session, thread, pipeline);
            this.sessions.Add(kcpSession.Conv, kcpSession);
            
            return true;
        }

        public void Close()
        {
            
        }



        public void OnTransportActive(IUdpSession session) => this.session = session;
        public void OnTransportInactive(IUdpSession session) => this.session = null;

        public void OnTransportException(IUdpSession session, Exception ex)
        {
            session.CloseAsync().NoAwait();
        }

        public void OnTransportRead(IUdpSession session, ref WrappedReader reader, WrappedArgs args)
        {
            var buffer = reader.ReadBuffer((int)reader.Length);
            var convBuffer = buffer.Slice(buffer.Start, 4);
            
            var conv = this.binaryOrderOperators.ReadUInt32(convBuffer.First.Span);
            if (!this.sessions.ContainsKey(conv)) return;

            var kcpSession = this.sessions[conv];
            if (kcpSession.RemoteAddress == session.RcvAddress)
            {
                kcpSession.ReceiveMessage(ref buffer);
            }
        }

        public void OnTransportWrite(IUdpSession session, ref WrappedWriter writer, WrappedArgs args)
        {
            
        }

        public override Task StopAsync()
        {
            this.controller.Dispose();
            return base.StopAsync();
        }
    }
}