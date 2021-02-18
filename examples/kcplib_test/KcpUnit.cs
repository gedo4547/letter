using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Threading;
using Letter.Kcp.lib__;
using KcpProject;


namespace kcplib_test
{
    public class KcpUnit : IDisposable
    {
        public KcpUnit()
        {
            this.kcp = new KcpProject.KCP(1, this.OnOutEvent);
            // this.kcp = new Kcp(1, true, SlabMemoryPoolFactory.Create(new MemoryPoolOptions(512, 16)), OnOutEvent);
            this.kcp.NoDelay(1, 10, 2, 1);
            this.kcp.SetStreamMode(false);
        
            // this.kcp.Flush(false);
            this.n_time = KcpHelper.currentMS();

            Thread thread = new Thread(OnUpdate);
            thread.Start();
        }
        


        private KCP kcp;

        // private Kcp kcp;
        private ConcurrentQueue<Memory<byte>> recv_queue = new ConcurrentQueue<Memory<byte>>();
        private ConcurrentQueue<byte[]> send_queue = new ConcurrentQueue<byte[]>();

        private volatile uint n_time;

        public Action<Memory<byte>> onRcvEvent;
        public Action<Memory<byte>> onSndEvent;

        public void Debug()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("等待发送的包：：" + send_queue.Count);
            sb.AppendLine("已经收到的包：："+recv_queue.Count);

            Console.WriteLine();
        }
        
        public void Recv(Memory<byte> memory)
        {
            // System.Random re = new Random();
            // int num = re.Next(0, 99);
            // if (num < 50)
            // {
            //     return;
            // }
            byte[] bytes = new byte[memory.Length];
            memory.CopyTo(bytes);
            recv_queue.Enqueue(bytes);
            this.n_time = KcpHelper.currentMS();
        }

        public void Send(byte[] bytes)
        {
            send_queue.Enqueue(bytes);
            this.n_time = KcpHelper.currentMS();
        }
        
        byte[] buffer = new byte[1400];

        public void OnUpdate(object obj)
        {
            while(true)
            {
                Thread.Sleep(1);

                if(KcpHelper.currentMS() >= this.n_time)
                {
                    while (this.recv_queue.Count > 0)
                    {
                       

                        this.recv_queue.TryDequeue(out var item);
                        // Console.WriteLine("xxxxxxxxxxxx>>"+item.Length);
                        var seg = item.GetBinaryArray();
                        kcp.Input(seg.Array, 0, seg.Count, true, false);
                        var length = this.kcp.PeekSize();
                        if (length < 0)
                        {
                            continue;
                        }
                        // Console.WriteLine(">>>>>>"+length);
                        
                        var bytesLength = this.kcp.Recv(buffer, 0, length);
                        Memory<byte> memory = new Memory<byte>(buffer, 0, bytesLength);

                        if(this.onRcvEvent != null)
                        {
                            this.onRcvEvent(memory);
                        }
                    }

                    while(this.send_queue.Count > 0)
                    {
                        if(this.kcp.WaitSnd > this.kcp.SndWnd * 2)
                        {
                            break;
                        }

                        this.send_queue.TryDequeue(out var item);

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