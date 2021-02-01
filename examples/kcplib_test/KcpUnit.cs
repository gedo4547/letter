using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Threading;
using Letter.Kcp.lib__;


namespace kcplib_test
{
    public class KcpUnit : IDisposable
    {
        public KcpUnit()
        {
            this.kcp = new Kcp(1, true, SlabMemoryPoolFactory.Create(new MemoryPoolOptions(512, 16)), OnOutEvent);
            this.kcp.NoDelay(1, 10, 2, 1);
            this.kcp.SetStreamMode(true);
            
            this.n_time = KcpHelper.currentMS();

            Thread thread = new Thread(OnUpdate);
            thread.Start();
        }
        
        private Kcp kcp;
        private ConcurrentQueue<Memory<byte>> recv_queue = new ConcurrentQueue<Memory<byte>>();
        private ConcurrentQueue<byte[]> send_queue = new ConcurrentQueue<byte[]>();

        private volatile uint n_time;

        public Action<Memory<byte>> onRcvEvent;
        public Action<Memory<byte>> onSndEvent;
        
        public void Recv(Memory<byte> bytes)
        {
            System.Random re = new Random();
            int num = re.Next(0, 99);
            if (num < 50)
            {
                return;
            }

            recv_queue.Enqueue(bytes);
            this.n_time = KcpHelper.currentMS();
        }

        public void Send(byte[] bytes)
        {
            send_queue.Enqueue(bytes);
            this.n_time = KcpHelper.currentMS();
        }
        
        
        private void OnUpdate(object obj)
        {
            while(true)
            {
                Thread.Sleep(0);

                if(KcpHelper.currentMS() >= this.n_time)
                {
                    while (this.recv_queue.TryDequeue(out var item))
                    {
                        // Console.WriteLine("xxxxxxxxxxxx>>"+item.Length);
                        var seg = item.GetBinaryArray();
                        kcp.Input(seg.Array, 0, seg.Count, true, false);
                        var length = this.kcp.PeekSize();
                        if (length < 0)
                        {
                            continue;
                        }
                        // Console.WriteLine(">>>>>>"+length);
                        byte[] bytes = new byte[length];
                        var bytesLength = this.kcp.Recv(bytes, 0, length);
                        Memory<byte> memory = new Memory<byte>(bytes, 0, bytesLength);

                        if(this.onRcvEvent != null)
                        {
                            this.onRcvEvent(memory);
                        }
                    }

                    while(this.send_queue.TryDequeue(out var item))
                    {
                        this.kcp.Send(item);
                    }
                    
                    this.kcp.Update();

                    this.n_time = this.kcp.Check();
                }
               
            }
        }
        
        private void OnOutEvent(byte[] bytes, int length)
        {
            var memory = new Memory<byte>(bytes, 0, length);

            if(this.onSndEvent != null)
            {
                this.onSndEvent(memory);
            }
        }

        public void Dispose()
        {
            
        }
    }
}