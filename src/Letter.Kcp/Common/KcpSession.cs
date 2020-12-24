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
            kcptun.Dispose();
            this.rcvMemoryOwner = this.MemoryPool.Rent();
            
            this.nextTime = TimeHelpr.GetNowTime().AddMilliseconds(options.interval);
            this.thread.Register(this);
        }
        
        public string Id { get; }
        public uint Conv { get; }
        public BinaryOrder Order { get; }
        public EndPoint LocalAddress { get; }
        public EndPoint RemoteAddress { get; }
        public IFilterPipeline<IKcpSession> Pipeline { get; }
        public PipeScheduler Scheduler => this.udpSession.Scheduler;
        public MemoryPool<byte> MemoryPool => this.udpSession.MemoryPool;
        
        private Kcptun kcptun;
        
        private IKcpThread thread;
        private IUdpSession udpSession;

        private DateTime nextTime;
        private IMemoryOwner<byte> rcvMemoryOwner;

        public void ReceiveMessage(ref ReadOnlySequence<byte> buffer)
        {
            this.kcptun.Input(buffer.First.ToMemory().Span);
            this.nextTime = TimeHelpr.GetNowTime();
            
            while (true)
            {
                int n = kcptun.PeekSize();
                if (n < 0)
                {
                    return;
                }
                if (n == 0)
                {
                    //Reset
                    return;
                }

                var span = rcvMemoryOwner.Memory.Span.Slice(0, n);
                int count = kcptun.Recv(span);
                if (n != count)
                {
                    return;
                }
                if (count <= 0)
                {
                    return;
                }
                
                // this.lastRecvTime = this.GetService().TimeNow;

                // this.OnRead(this.memoryStream);
            }
        }
        
        public void Write(object o)
        {
            WrappedWriter writer = new WrappedWriter();
            // this.kcptun.Send()
            throw new System.NotImplementedException();
        }

        public Task FlushAsync()
        {
            throw new System.NotImplementedException();
        }
        
        public void Output(IMemoryOwner<byte> buffer, int avalidLength)
        {
            throw new System.NotImplementedException();
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