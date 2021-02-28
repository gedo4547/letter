using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using Letter.IO;
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

            var littleEndian = this.Order == BinaryOrder.LittleEndian;
            this.kcpKit = new KcpKit(conv, littleEndian, this.MemoryPool);
            this.kcpKit.SettingMtu(options.Mtu);
            this.kcpKit.SettingNoDelay(options.NoDelay);
            this.kcpKit.SettingWndSize(options.WndSize);
            this.kcpKit.SettingStreamMode(options.StreamMode);
            this.kcpKit.SettingReservedSize(options.ReservedSize);

            this.kcpKit.onRcv += this.OnKcpRcvEvent;
            this.kcpKit.onSnd += this.OnKcpSndEvent;

            this.kcpOperators = BinaryOrderOperatorsFactory.GetOperators(this.Order);
            this.rcvPool = new WrappedMemoryPool(this.MemoryPool, MemoryFlag.Kcp);
            this.sndPool = new WrappedMemoryPool(this.MemoryPool, MemoryFlag.Kcp);

            this.sndMemory = new WrappedMemory(this.MemoryPool.Rent(), MemoryFlag.Kcp);
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
        private IBinaryOrderOperators kcpOperators;

        private WrappedMemory readerUdpMemory;
        private WrappedMemory writerUdpMemory;

        private ReaderFlushDelegate readerFlushDelegate;
        private WriterFlushDelegate writerFlushDelegate;
        private RunnableUnitDelegate runnableUnitDelegate;
        
        private volatile bool isClosed = false;

        private object rcv_sync = new object();
        private object snd_sync = new object();


        private WrappedMemoryPool rcvPool;
        private WrappedMemoryPool sndPool;
        private ConcurrentQueue<WrappedMemory> rcvQueue = new ConcurrentQueue<WrappedMemory>();
        private ConcurrentQueue<WrappedMemory> sndQueue = new ConcurrentQueue<WrappedMemory>();
       
        private void Update()
        {
            if (this.isClosed) return;

            while (this.rcvQueue.Count > 0)
            {
                this.rcvQueue.TryPeek(out var item);
                var seg = item.GetReadableMemory().GetBinaryArray();
                if (!this.kcpKit.TryRcv(seg.Array, seg.Offset, seg.Count, item.Regular))
                {
                    break;
                }
                else
                {
                    this.rcvQueue.TryDequeue(out _);
                    this.rcvPool.Push(item);
                    this.kcpKit.MarkTime();
                }
            }

            while (this.sndQueue.Count > 0)
            {
                this.sndQueue.TryPeek(out var item);
                var readableMemory = item.GetReadableMemory();
                var seg = readableMemory.GetBinaryArray();
                if (!this.kcpKit.TrySnd(seg.Array, seg.Offset, seg.Count))
                {
                    break;
                }
                else
                {
                    //Console.WriteLine("kcp.send完成");
                    this.sndQueue.TryDequeue(out _);
                    this.sndPool.Push(item);
                    this.kcpKit.MarkTime();
                }
            }

            this.kcpKit.Update();
        }
        
        public void InputKcpMessage(ref ReadOnlySequence<byte> buffer, bool regular = true)
        {
            if (this.isClosed)
            {
                return;
            }

            var memory = this.rcvPool.Pop();
            var writableMemory = memory.GetWritableMemory((int)buffer.Length);
            buffer.CopyTo(writableMemory.Span);
            memory.WriterAdvance((int)buffer.Length);
            memory.Regular = regular;
            this.rcvQueue.Enqueue(memory);
        }

        public void InputUdpMessage(ref ReadOnlySequence<byte> buffer)
        {
            var udpBuffer = buffer.Slice(buffer.GetPosition(4));
            var reader = new WrappedReader(udpBuffer, this.Order, this.readerFlushDelegate);
            this.Pipeline.OnTransportRead(this, ref reader);
            reader.Flush();
        }

        private void OnKcpRcvEvent(uint conv, ref ReadOnlySequence<byte> sequence)
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

        public void SendReliableAsync(object o)
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

        public void SendUnreliableAsync(object o)
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
                    //conv使用kcp的序列写入，保证与kcp一致
                    var span = this.writerUdpMemory.GetWritableSpan(4);
                    this.kcpOperators.WriteUInt32(span, this.Conv);
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
                //Console.WriteLine("kcp数据写入完成》》" + readableMemory.Length);
                this.sndQueue.Enqueue(memory);
            }
            else if(memory.Flag == MemoryFlag.Udp)
            {
                this.udpSession.Write(this.RemoteAddress, memory);
                this.udpSession.FlushAsync().NoAwait();
            }
        }

        private WrappedMemory sndMemory;

        private void OnKcpSndEvent(uint conv, ref ReadOnlySequence<byte> memory)
        {
            //Console.WriteLine("kcp通知消息发送>>"+memory.Length);
            if (memory.Length < 1) return;

            int length = (int)memory.Length;
            sndMemory.Clear();
            var writableMemory = this.sndMemory.GetWritableMemory(length);
            memory.CopyTo(writableMemory.Span);
            this.sndMemory.WriterAdvance(length);

            this.udpSession.Write(this.RemoteAddress, this.sndMemory);
            this.udpSession.FlushAsync().NoAwait();
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
            
            //this.readerKcpMemory.Dispose();
            //this.writerKcpMemory.Dispose();

            this.closable.OnSessionClosed(this);
            
            return Task.CompletedTask;
        }
    }
}