using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using FilterGroup = Letter.DgramChannelFilterGroup<Letter.Udp.IUdpSession, Letter.Udp.IUdpChannelFilter>;


namespace Letter.Udp
{
    partial class UdpSession : IUdpSession
    {
        public UdpSession(Socket socket, UdpOptions options, MemoryPool<byte> memoryPool, PipeScheduler scheduler, FilterGroup filterGroup)
        {
            this.Id = IdGeneratorHelper.GetNextId();
            
            this.order = options.Order;
            this.Scheduler = scheduler;
            this.MemoryPool = memoryPool;
            this.filterGroup = filterGroup;
            this.LoaclAddress = this.udpSocket.LocalAddress;
            
            this.onMemoryWritePush = this.OnMemoryWritePush;
            this.senderPipeline = new DgramPipeline(this.MemoryPool, this.Scheduler, this.OnSenderPipelineReceiveBuffer);
            this.receiverPipeline = new DgramPipeline(this.MemoryPool, this.Scheduler, this.OnReceiverPipelineReceiveBuffer);
            
            this.udpSocket = new UdpSocket(socket, scheduler);
            if (options.RcvTimeout != null)
                udpSocket.SettingRcvTimeout(options.RcvTimeout.Value);
            if (options.SndTimeout != null)
                udpSocket.SettingSndTimeout(options.SndTimeout.Value);
            
            if (options.RcvBufferSize != null)
                udpSocket.SettingRcvBufferSize(options.RcvBufferSize.Value);
            if (options.SndBufferSize != null)
                udpSocket.SettingSndBufferSize(options.SndBufferSize.Value);
        }
        
        public string Id { get; private set; }
        public EndPoint LoaclAddress { get; private set; }
        
        public EndPoint RcvAddress { get; private set; }
        public EndPoint SndAddress { get; private set; }
        
        public PipeScheduler Scheduler { get; private set; }
        public MemoryPool<byte> MemoryPool { get; private set; }
        
        public EndPoint RemoteAddress
        {
            get { throw new Exception("please use IUdpSession.RcvAddress or IUdpSession.SndAddress"); }
        }

        private UdpSocket udpSocket;
        private DgramPipeline senderPipeline;
        private DgramPipeline receiverPipeline;

        private BinaryOrder order;
        private FilterGroup filterGroup;
        private WrappedDgramWriter.MemoryWritePushDelegate onMemoryWritePush;

        private Task memoryTask;

        protected volatile bool isDisposed = false;
        
        private object writerSync = new object();
        
        public void StartAsync()
        {
            this.StartReceiveSenderPipelineBuffer();
            this.StartReceiveReceiverPipelineBuffer();
            
            this.memoryTask = this.SocketReceiveAsync();
            
            this.filterGroup.OnChannelActive(this);
        }
        
        public Task WriteAsync(EndPoint remoteAddress, object obj)
        {
            lock (writerSync)
            {
                if (this.isDisposed)
                {
                    throw new ObjectDisposedException("UdpSession has been released");
                }
                
                return this.WriteBufferAsync(remoteAddress, obj);
            }
        }

        public Task WriteAsync(EndPoint remoteAddress, ref ReadOnlySequence<byte> sequence)
        {
            lock (writerSync)
            {
                if (this.isDisposed)
                {
                    throw new ObjectDisposedException("UdpSession has been released");
                }
                
                return this.WriteBufferAsync(remoteAddress, ref sequence);
            }
        }
        
        public async ValueTask DisposeAsync()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.isDisposed = true;
            
            this.filterGroup.OnChannelInactive(this);

            this.Id = string.Empty;
            
            await this.udpSocket.DisposeAsync();
            
            if (this.memoryTask != null)
            {
                await this.memoryTask;
            }
            
            if (this.senderPipeline != null)
            {
                this.senderPipeline.Dispose();
            }

            if (this.receiverPipeline != null)
            {
                this.receiverPipeline.Dispose();
            }

            if (this.filterGroup != null)
            {
                await this.filterGroup.DisposeAsync();
            }

            if (this.onMemoryWritePush != null)
            {
                this.onMemoryWritePush = null;
            }
            
            if (this.MemoryPool != null)
            {
                this.MemoryPool.Dispose();
            }
        }
    }
}