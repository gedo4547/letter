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
    sealed class KcpChannel : AChannel<IKcpSession, KcpOptions>, IKcpChannel, IFilter<IUdpSession>, IKcpClosable
    {
        public KcpChannel(KcpOptions options, IUdpChannel channel, IKcpThread thread, Action<IFilterPipeline<IKcpSession>> handler)
        {
            base.ConfigurationSelfOptions(options);
            base.ConfigurationSelfFilter(handler);
            
            var order = KcpHelpr.KcpGlobalBinaryOrder;
            this.binaryOrderOperators = BinaryOrderOperatorsFactory.GetOperators(order);

            this.thread = thread;
            this.channel = channel;
            this.channel.ConfigurationSelfFilter((pipeline) => { pipeline.Add(this); });
        }

        private IUdpChannel channel;
        private IUdpSession session;
        
        private IKcpThread thread;
        private IBinaryOrderOperators binaryOrderOperators;
        
        private bool isStop = false;
        private bool isInvalid = false;

        private Dictionary<uint, KcpSession> sessions = new Dictionary<uint, KcpSession>();
        
        public async Task BindAsync(EndPoint address)
        {
            await this.channel.StartAsync(address);
        }
        
        public bool Connect(uint conv, EndPoint remoteAddress)
        {
            if(this.isStop)
                throw new Exception("The channel has stopped working");
            if(this.isInvalid)
                throw new Exception("Channel failure, call the StopAsync method to release the channel");

            if (this.sessions.ContainsKey(conv)) return false;

            var localAddress = this.session.LocalAddress;
            var pipeline = base.CreateFilterPipeline();
            var kcpSession = new KcpSession(conv, remoteAddress, localAddress, options, session, thread, pipeline, this);

            this.sessions.Add(kcpSession.CurrentConv, kcpSession);
            
            return true;
        }

        public void Close(uint conv)
        {
            if(!this.sessions.ContainsKey(conv))
            {
                return;
            }

            this.sessions.Remove(conv);
        }

        public void OnTransportActive(IUdpSession session) => this.session = session;
        public void OnTransportInactive(IUdpSession session) => this.session = null;

        public void OnTransportException(IUdpSession session, Exception ex)
        {
            this.isInvalid = true;

            foreach (var item in this.sessions)
            {
                item.Value.OnUdpMessageException(ex);
            }
        }

        public void OnTransportRead(IUdpSession session, ref WrappedReader reader, WrappedArgs args)
        {
            if(this.isInvalid || this.isStop) return;

            var buffer = reader.ReadBuffer((int)reader.Length);
            var convSpan = buffer.Slice(buffer.Start, 4).First.Span;
            
            var conv = this.binaryOrderOperators.ReadUInt32(convSpan);
            if (!this.sessions.ContainsKey(conv)) 
            {
                return;
            }

            this.sessions[conv].ReceiveMessage(ref buffer);
        }

        public void OnTransportWrite(IUdpSession session, ref WrappedWriter writer, WrappedArgs args)
        {
            if(this.isInvalid || this.isStop) return;

            WrappedMemory memory = args.Value as WrappedMemory;
            var readableMemory = memory.GetReadableMemory();

            writer.Write(readableMemory);
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

            this.sessions.Clear();
        }
    }
}