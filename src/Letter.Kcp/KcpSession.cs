using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets.Kcp;
using System.Threading.Tasks;

using Letter.IO;
using Letter.Udp;

namespace Letter.Kcp
{
    sealed class KcpSession : IKcpSession, IKcpCallback, IRentable
    {
        public KcpSession(uint conv, EndPoint remoteAddress, EndPoint localAddress, KcpOptions options, IUdpSession udpSession, IEventSubscriber subscriber, FilterPipeline<IKcpSession> pipeline, IKcpClosable closable)
        {
            this.Id = IdGeneratorHelper.GetNextId();
            this.Conv = conv;
            this.LocalAddress = localAddress;
            this.RemoteAddress = remoteAddress;

            this.Order = options.Order;
            this.udpSession = udpSession;
            this.subscriber = subscriber;
            this.Pipeline = pipeline;
            this.closable = closable;

            this.kcplib = new KcpImpl(conv, this, this);
            this.kcplib.SetMtu(options.Mtu);
            this.kcplib.SetNoDelay(options.NoDelay);
            this.kcplib.SetWndSize(options.WndSize);
            this.kcplib.Interval(options.interval);

            this.readerKcpMemory = new WrappedMemory(this.MemoryPool.Rent(), MemoryFlag.Kcp);
            this.writerKcpMemory = new WrappedMemory(this.MemoryPool.Rent(), MemoryFlag.Kcp);

            this.readerUdpMemory = new WrappedMemory(this.MemoryPool.Rent(), MemoryFlag.Udp);
            this.writerUdpMemory = new WrappedMemory(this.MemoryPool.Rent(), MemoryFlag.Udp);

            this.readerFlushDelegate = (pos, endPos) => { };
            this.writerFlushDelegate = this.OnWriterComplete;
            
            this.nextTime = TimeHelpr.GetNowTime().AddMilliseconds(options.interval);
            this.runnableUnitDelegate = this.Update;
            this.subscriber.Register(this.runnableUnitDelegate);

            this.Pipeline.OnTransportActive(this);
        }
        
        public string Id { get; }
        public uint Conv {get;}
        public BinaryOrder Order { get; }
        public EndPoint LocalAddress { get; }
        public EndPoint RemoteAddress { get; }
        public FilterPipeline<IKcpSession> Pipeline { get; }
        public PipeScheduler Scheduler => this.udpSession.Scheduler;
        public MemoryPool<byte> MemoryPool => this.udpSession.MemoryPool;
        
        private KcpImpl kcplib;
        private IEventSubscriber subscriber;
        private IUdpSession udpSession;
        private IKcpClosable closable;
        
        private WrappedMemory readerKcpMemory;
        private WrappedMemory writerKcpMemory;

        private WrappedMemory readerUdpMemory;
        private WrappedMemory writerUdpMemory;

        private ReaderFlushDelegate readerFlushDelegate;
        private WriterFlushDelegate writerFlushDelegate;
        private RunnableUnitDelegate runnableUnitDelegate;
        
        private DateTime nextTime;
        private volatile bool isClosed = false;
        private object sync = new object();
       
        private void Update(ref DateTime nowTime)
        {
            if (this.isClosed) return;
            if (nowTime < this.nextTime) return;
            
            this.kcplib.Update(nowTime);

            this.ReadKcpMessage();
            
            this.nextTime = this.kcplib.Check(nowTime);
        }
        
        public void InputKcpMessage(ref ReadOnlySequence<byte> buffer)
        {
            if (this.isClosed)
            {
                return;
            }
            
            var errorCode = this.kcplib.Input(buffer.First.ToMemory().Span);
            if (errorCode !=  0)
            {
                this.DeliverException(new KcpException(errorCode));
                this.CloseAsync().NoAwait();
                return;
            }
            
            this.nextTime = TimeHelpr.GetNowTime();
        }

        public void InputUdpMessage(ref ReadOnlySequence<byte> buffer)
        {
            var reader = new WrappedReader(buffer, this.Order, this.readerFlushDelegate);
            this.Pipeline.OnTransportRead(this, ref reader);
            reader.Flush();
        }

        private void ReadKcpMessage()
        {
            while (true)
            {
                int size = this.kcplib.PeekSize();
                if (size <= 0) return;
                
                int count = this.kcplib.Recv(this.readerKcpMemory.GetWritableSpan(size));
                this.readerKcpMemory.WriterAdvance(count);
                
                if (size != count) return;
                if (count <= 0) return;

                try
                {
                    var sequence = readerKcpMemory.GetReadableMemory().ToSequence();
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

        public void SafeSendAsync(object o)
        {
            lock (sync)
            {
                if(this.isClosed)
                {
                    return;
                }

                try
                {
                    var writer = new WrappedWriter(this.writerKcpMemory, this.Order, this.writerFlushDelegate);
                    this.Pipeline.OnTransportWrite(this, ref writer, o);
                    writer.Flush();
                }
                catch (Exception e)
                {
                    this.DeliverException(e);
                }
            }
        }

        public void UnsafeSendAsync(EndPoint remoteAddress, object o)
        {
            lock (this.sync)
            {
                if (this.isClosed)
                {
                    return;
                }

                try
                {
                    this.writerUdpMemory.Token = remoteAddress;
                    var writer = new WrappedWriter(this.writerUdpMemory, this.Order, this.writerFlushDelegate);
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
            if (memory.Flag == MemoryFlag.Kcp)
            {
                var readableMemory = memory.GetReadableMemory();
                if (readableMemory.Length < 1)
                {
                    return;
                }

                this.kcplib.Send(readableMemory.Span);
                this.nextTime = TimeHelpr.GetNowTime();
            }
            else if(memory.Flag == MemoryFlag.Udp)
            {
                var remoteAddress = memory.Token as EndPoint;
                this.udpSession.Write(remoteAddress, memory);
                this.udpSession.FlushAsync().NoAwait();
            }
        }

        private WrappedMemory sndMemory = new WrappedMemory(MemoryFlag.Kcp);
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

        public void OnUdpMessageException(Exception ex)
        {
            this.DeliverException(ex);
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

            this.subscriber.Unregister(this.runnableUnitDelegate);
            this.Pipeline.OnTransportInactive(this);
            
            this.kcplib.Dispose();
            this.kcplib = null;

            this.readerKcpMemory.Dispose();
            this.writerKcpMemory.Dispose();

            this.closable.OnSessionClosed(this);
            
            return Task.CompletedTask;
        }
    }
}