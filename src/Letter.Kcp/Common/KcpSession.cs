using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets.Kcp;
using System.Threading.Tasks;

using Letter.IO;
using Letter.Udp;

using Kcplib = System.Net.Sockets.Kcp.Kcp;

namespace Letter.Kcp
{
    sealed class KcpSession : IKcpSession, IKcpCallback, IRentable, IKcpRunnable
    {
        public KcpSession(uint conv, EndPoint remoteAddress, EndPoint localAddress, KcpOptions options, IUdpSession udpSession, IKcpThread thread, FilterPipeline<IKcpSession> pipeline)
        {
            this.Id = IdGeneratorHelper.GetNextId();
            this.Conv = conv;
            
            this.LocalAddress = localAddress;
            this.RemoteAddress = remoteAddress;

            this.Order = options.Order;
            this.udpSession = udpSession;
            this.thread = thread;
            this.Pipeline = pipeline;

            this.kcplib = new Kcplib(conv, this, this);
            this.kcplib.SetMtu(options.Mtu);
            this.kcplib.SetNoDelay(options.NoDelay);
            this.kcplib.SetWndSize(options.WndSize);
            this.kcplib.Interval(options.interval);
            this.readerMemory = new WrappedMemory(this.MemoryPool.Rent());
            this.writerMemory = new WrappedMemory(this.MemoryPool.Rent());
            this.readerFlushDelegate = (pos, endPos) => { };
            this.writerFlushDelegate = this.OnWriterComplete;
            
            this.nextTime = TimeHelpr.GetNowTime().AddMilliseconds(options.interval);
            this.thread.Register(this);
            
            this.Pipeline.OnTransportActive(this);
        }
        
        public string Id { get; }
        public uint Conv { get; }
        public BinaryOrder Order { get; }
        public EndPoint LocalAddress { get; }
        public EndPoint RemoteAddress { get; }
        public FilterPipeline<IKcpSession> Pipeline { get; }
        public PipeScheduler Scheduler => this.udpSession.Scheduler;
        public MemoryPool<byte> MemoryPool => this.udpSession.MemoryPool;
        
        private Kcplib kcplib;
        private IKcpThread thread;
        private IUdpSession udpSession;
        
        private WrappedMemory readerMemory;
        private WrappedMemory writerMemory;
        private ReaderFlushDelegate readerFlushDelegate;
        private WriterFlushDelegate writerFlushDelegate;
        
        private DateTime nextTime;
        private volatile bool isClosed = false;
        private object sync = new object();
       
        public void Update(ref DateTime nowTime)
        {
            if (this.isClosed) return;

            if (nowTime < this.nextTime) return;
            
            this.kcplib.Update(nowTime);

            this.ReceiveKcpMessage();
            
            this.nextTime = this.kcplib.Check(nowTime);
        }
        
        public void ReceiveMessage(ref ReadOnlySequence<byte> buffer)
        {
            if (this.isClosed)
            {
                return;
            }
            
            var errorCode = this.kcplib.Input(buffer.First.ToMemory().Span);
            if (errorCode !=  0)
            {
                this.CloseAsync().NoAwait();
                return;
            }
            
            this.nextTime = TimeHelpr.GetNowTime();
        }

        private void ReceiveKcpMessage()
        {
            while (true)
            {
                int size = this.kcplib.PeekSize();
                if (size <= 0) return;
                
                int count = this.kcplib.Recv(this.readerMemory.GetWritableSpan(size));
                this.readerMemory.WriterAdvance(count);
                
                if (size != count) return;
                if (count <= 0) return;

                try
                {
                    var sequence = readerMemory.GetReadableMemory().ToSequence();
                    var reader = new WrappedReader(sequence, this.Order, this.readerFlushDelegate);
                    this.Pipeline.OnTransportRead(this, ref reader);
                    reader.Flush();
                }
                catch (Exception e)
                {
                    this.DeliverException(e);
                    return;
                }
            }
        }

        public void Send(object o)
        {
            lock (sync)
            {
                try
                {
                    var writer = new WrappedWriter(this.writerMemory, this.Order, this.writerFlushDelegate);
                    this.Pipeline.OnTransportWrite(this, ref writer, o);
                    writer.Flush();
                }
                catch (Exception e)
                {
                    this.DeliverException(e);
                }
            }
        }

        private void OnWriterComplete(IWrappedWriter writer)
        {
            WrappedMemory memory = writer as WrappedMemory;
            var readableMemory = memory.GetReadableMemory();
            if (readableMemory.Length < 1)
            {
                return;
            }

            this.kcplib.Send(readableMemory.Span);
            this.nextTime = TimeHelpr.GetNowTime();
        }


        private WrappedMemory sndMemory = new WrappedMemory();
        public void Output(IMemoryOwner<byte> buffer, int avalidLength)
        {
            if (avalidLength < 1) return;
            
            this.sndMemory.SettingMemory(buffer, avalidLength);
            this.udpSession.Write(this.RemoteAddress, sndMemory);
            this.udpSession.FlushAsync().NoAwait();
        }
        
        public IMemoryOwner<byte> RentBuffer(int length)
        {
            return null;
        }
        
        private void DeliverException(Exception ex)
        {
            this.Pipeline.OnTransportException(this, ex);
            this.CloseAsync().NoAwait();
        }

        public Task CloseAsync()
        {
            if (this.isClosed)
            {
                return Task.CompletedTask;
            }

            this.isClosed = true;
            this.Pipeline.OnTransportInactive(this);
            this.thread.Unregister(this);
            
            this.kcplib.Dispose();
            this.kcplib = null;
            this.readerMemory.Dispose();
            this.writerMemory.Dispose();
            
            return Task.CompletedTask;
        }
    }
}