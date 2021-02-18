using System;
using System.Buffers;
using System.Collections.Concurrent;
using Letter.Kcp.lib__;

namespace kcp_lab_test
{
    public class KcpUnit
    {
        public KcpUnit(MemoryPool<byte> memoryPool)
        {
            this._kcpKit = new KcpKit(1, true, memoryPool);
            this._kcpKit.SettingNoDelay(1, 10, 2, 1);
            this._kcpKit.WriteDelay = false;
            this._kcpKit.SettingStreamMode(false);

            System.Threading.Thread thread = new System.Threading.Thread(()=> 
            {
                this.Update();
            });
            thread.Start();
        }

        private KcpKit _kcpKit;

        private ConcurrentQueue<Memory<byte>> recv_queue = new ConcurrentQueue<Memory<byte>>();
        private ConcurrentQueue<Memory<byte>> send_queue = new ConcurrentQueue<Memory<byte>>();

        public void SetRcvEvent(RefAction rcv)
        {
            this._kcpKit.onRcv += rcv;
        }

        public void SetSndEvent(RefAction snd)
        {
            this._kcpKit.onSnd += snd;
        }

        public void Recv(Memory<byte> data)
        {
            this.recv_queue.Enqueue(data);
            this._kcpKit.SetNextTime(KcpHelper.currentMS());
        }

        int num;
        public void Send(Memory<byte> data)
        {
            num++;
            Console.WriteLine("Send>>"+num);
            this.send_queue.Enqueue(data);
            this._kcpKit.SetNextTime(KcpHelper.currentMS());
        }

        public void Update()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(1);

                while (this.recv_queue.Count > 0)
                {
                    this.recv_queue.TryPeek(out var item);
                    var seg = item.GetBinaryArray();
                    var length = this._kcpKit.Recv(seg.Array, 0, seg.Count);
                    if (length < 0)
                    {
                        this.recv_queue.TryDequeue(out _);
                    }
                }

                while (this.send_queue.Count > 0)
                {
                    this.send_queue.TryPeek(out var item);
                    var seg = item.GetBinaryArray();
                    //Console.WriteLine(">>>>>>>>");
                    var length = this._kcpKit.Send(seg.Array, 0, seg.Count);
                    if (length != 0)
                    {
                        this.send_queue.TryDequeue(out _);
                    }
                }

                this._kcpKit.Update();
            }
            
        }
    }
}