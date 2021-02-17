using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

using Letter.IO;
using Letter.IO.Kcplib;

using Letter.Udp;

namespace Letter.Kcp
{
    sealed class KcpSession : IKcpSession
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

            this.kcpKit = new KcpKit(conv);

            if(options.Mtu != null)
                this.kcpKit.SettingMtu(options.Mtu.Value);

            this.kcpKit.SettingWndSize(
                options.WndSize.sndwnd, 
                options.WndSize.rcvwnd);

            this.kcpKit.SettingNoDelay(
                options.NoDelay.nodelay_, 
                options.NoDelay.interval_, 
                options.NoDelay.resend_, 
                options.NoDelay.nc_);

            if(options.StreamMode != null)
                this.kcpKit.SettingStreamMode(options.StreamMode.Value);

            if(options.ReservedSize != null)
                this.kcpKit.SettingReserveBytes(options.ReservedSize.Value);

            this.kcpKit.onRcv += this.OnKcpRcvEvent;
            this.kcpKit.onSnd += this.OnKcpSndEvent;

            this.rcvPool = new WrappedMemoryPool(this.MemoryPool, MemoryFlag.Kcp);
            this.sndPool = new WrappedMemoryPool(this.MemoryPool, MemoryFlag.Kcp);


            this.readerKcpMemory = new WrappedMemory(this.MemoryPool.Rent(), MemoryFlag.Kcp);
            this.writerKcpMemory = new WrappedMemory(this.MemoryPool.Rent(), MemoryFlag.Kcp);

            this.readerUdpMemory = new WrappedMemory(this.MemoryPool.Rent(), MemoryFlag.Udp);
            this.writerUdpMemory = new WrappedMemory(this.MemoryPool.Rent(), MemoryFlag.Udp);

            this.readerFlushDelegate = (pos, endPos) => { };
            this.writerFlushDelegate = this.OnWriterComplete;
            
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
        public PipeScheduler Scheduler { get { return this.udpSession.Scheduler; } }
        public MemoryPool<byte> MemoryPool { get { return this.udpSession.MemoryPool; } }
        

        private KcpKit kcpKit;
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

        private object rcv_sync = new object();
        private object snd_sync = new object();


        private WrappedMemoryPool rcvPool;
        private WrappedMemoryPool sndPool;
        private Queue<WrappedMemory> rcvQueue = new Queue<WrappedMemory>();
        private Queue<WrappedMemory> sndQueue = new Queue<WrappedMemory>();
       
        private void Update(ref DateTime nowTime)
        {
            if (this.isClosed) return;

            lock(this.rcv_sync)
            {
                while(this.rcvQueue.Count > 0)
                {
                    var item = this.rcvQueue.Peek();
                    var seg = item.GetReadableMemory().GetBinaryArray();
                    var opencode = this.kcpKit.Recv(seg.Array, seg.Offset, seg.Count);

                    if(opencode < 0) break;

                    this.rcvQueue.Dequeue();
                    this.sndPool.Push(item);
                }
            }
            
            lock(this.snd_sync)
            {
                while(this.sndQueue.Count > 0)
                {
                    var item = this.sndQueue.Peek();
                    var seg = item.GetReadableMemory().GetBinaryArray();
                    var opencode = this.kcpKit.Send(seg.Array, seg.Offset, seg.Count);

                    if(opencode == 0) break;

                    this.sndQueue.Dequeue();
                    this.sndPool.Push(item);
                }
            }
            

            this.kcpKit.Update();
        }
        
        public void InputKcpMessage(ref ReadOnlySequence<byte> buffer)
        {
            if (this.isClosed)
            {
                return;
            }

            lock(this.rcv_sync)
            {
                var memory = this.rcvPool.Pop();
                var writableMemory = memory.GetWritableMemory((int)buffer.Length);
                buffer.CopyTo(writableMemory.Span);
                memory.WriterAdvance((int)buffer.Length);
                this.rcvQueue.Enqueue(memory);
            }
        }

        public void InputUdpMessage(ref ReadOnlySequence<byte> buffer)
        {
            var udpBuffer = buffer.Slice(buffer.GetPosition(4));
            var reader = new WrappedReader(udpBuffer, this.Order, this.readerFlushDelegate);
            this.Pipeline.OnTransportRead(this, ref reader);
            reader.Flush();
        }

        private void OnKcpRcvEvent(ref ReadOnlySequence<byte> sequence)
        {
            try
            {
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

        

        public void SafeSendAsync(object o)
        {
            lock (this.snd_sync)
            {
                if(this.isClosed)
                {
                    return;
                }

                try
                {
                    var memory = this.sndPool.Pop();
                    var writer = new WrappedWriter(memory, this.Order, this.writerFlushDelegate);
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
            lock (this.snd_sync)
            {
                if (this.isClosed)
                {
                    return;
                }

                try
                {
                    this.writerUdpMemory.Clear();
                    this.writerUdpMemory.Token = remoteAddress;
                    //conv使用kcp的序列写入，保证与kcp一致
                    var span = this.writerUdpMemory.GetWritableSpan(4);
                    KcpHelpr.GetOperators().WriteUInt32(span, this.Conv);
                    writerUdpMemory.WriterAdvance(4);

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


        int num = 0;
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
                this.sndQueue.Enqueue(memory);
            }
            else if(memory.Flag == MemoryFlag.Udp)
            {
                var remoteAddress = memory.Token as EndPoint;
                this.udpSession.Write(remoteAddress, memory);
                this.udpSession.FlushAsync().NoAwait();
            }
        }

        int num1 = 0;
        private WrappedMemory sndMemory = new WrappedMemory(MemoryFlag.Kcp);
        public void Output(IMemoryOwner<byte> buffer, int avalidLength)
        {
            System.Threading.Interlocked.Increment(ref num1);

            Console.WriteLine("kcp发送>>>" + avalidLength+"             num::"+num1);
            if (avalidLength < 1) return;
            
            this.sndMemory.SettingMemory(buffer, avalidLength);
            this.udpSession.Write(this.RemoteAddress, sndMemory);
            this.udpSession.FlushAsync().NoAwait();
        }

        private void OnKcpSndEvent(ref ReadOnlySequence<byte> memory)
        {
            if (memory.Length < 1) return;


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
            
            this.readerKcpMemory.Dispose();
            this.writerKcpMemory.Dispose();

            this.closable.OnSessionClosed(this);
            
            return Task.CompletedTask;
        }
    }
}