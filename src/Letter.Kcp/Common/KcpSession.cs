using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets.Kcp;
using System.Threading.Tasks;

using Letter.IO;
using Letter.Udp;

using Kcptun = System.Net.Sockets.Kcp.Kcp;

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

            this.kcptun = new Kcptun(conv, this, this);
            this.kcptun.SetMtu(options.Mtu);
            this.kcptun.SetNoDelay(options.NoDelay);
            this.kcptun.SetWndSize(options.WndSize);
            this.kcptun.Interval(options.interval);
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
        
        private Kcptun kcptun;
        private IKcpThread thread;
        private IUdpSession udpSession;
        
        private WrappedMemory readerMemory;
        private WrappedMemory writerMemory;
        private ReaderFlushDelegate readerFlushDelegate;
        private WriterFlushDelegate writerFlushDelegate;
        
        private DateTime nextTime;
        private bool isClosed = false;
        private object sync = new object();
        
        public void ReceiveMessage(ref ReadOnlySequence<byte> buffer)
        {
            if (this.isClosed)
            {
                return;
            }
            
            this.kcptun.Input(buffer.First.ToMemory().Span);
            this.nextTime = TimeHelpr.GetNowTime();
            
            while (true)
            {
                int n = this.kcptun.PeekSize();
                if (n < 0) return;
                
                if (n == 0)
                {
                    //Reset
                    return;
                }

                // Console.WriteLine("kcp收到数据::::" + n);
                int count = this.kcptun.Recv(this.readerMemory.GetWritableSpan(n));
                this.readerMemory.WriterAdvance(count);
                
                if (n != count) return;
                if (count <= 0) return;

                var sequence = readerMemory.GetReadableMemory().ToSequence();
                // Console.WriteLine("得到kcp处理后的数据::::" + sequence.Length);
                var reader = new WrappedReader(sequence, this.Order, (pos, endPos) => { });
                this.Pipeline.OnTransportRead(this, ref reader);
                reader.Flush();
            }
        }
        
        public void Send(object o)
        {
            lock (sync)
            {
                var writer = new WrappedWriter(this.writerMemory, this.Order, this.writerFlushDelegate);
                this.Pipeline.OnTransportWrite(this, ref writer, o);
                writer.Flush();
            }
        }

        private void OnWriterComplete(IWrappedWriter writer)
        {
            WrappedMemory memory = writer as WrappedMemory;
            var readableMemory = memory.GetReadableMemory();
            // Console.WriteLine("kcp数据写入完成》》》》" + readableMemory.Length);
            if (readableMemory.Length < 1)
            {
                return;
            }

            this.kcptun.Send(readableMemory.Span);
            this.nextTime = TimeHelpr.GetNowTime();
        }


        private WrappedMemory sndMemory = new WrappedMemory();
        public void Output(IMemoryOwner<byte> buffer, int avalidLength)
        {
            // Console.WriteLine("kcp抛出发送事件》长度》" + avalidLength + "      目标：" + this.RemoteAddress.ToString());
            this.sndMemory.SettingMemory(buffer, avalidLength);
            this.udpSession.Write(this.RemoteAddress, sndMemory);
            this.udpSession.FlushAsync().NoAwait();
        }
        
        public IMemoryOwner<byte> RentBuffer(int length)
        {
            return null;
        }
        
        public void Update(ref DateTime nowTime)
        {
            if (nowTime < this.nextTime) return;
            
            this.kcptun.Update(nowTime);
            
            this.nextTime = this.kcptun.Check(nowTime);
        }
        
        public Task CloseAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}