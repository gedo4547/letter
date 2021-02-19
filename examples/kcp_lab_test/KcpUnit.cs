using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Net;

namespace kcp_lab_test
{
    public class KcpUnit
    {
        public KcpUnit(MemoryPool<byte> memoryPool)
        {
            this._kcpKit = new KcpKit(1, true, memoryPool);
            this._kcpKit.SettingNoDelay(1, 10, 2, 1);
            this._kcpKit.WriteDelay = true;
            this._kcpKit.SettingStreamMode(true);

            System.Threading.Thread thread = new System.Threading.Thread(()=> 
            {
                this.Update();
            });
            thread.Start();
        }

        private KcpKit _kcpKit;

        private ConcurrentQueue<Memory<byte>> recv_queue = new ConcurrentQueue<Memory<byte>>();
        private ConcurrentQueue<Memory<byte>> send_queue = new ConcurrentQueue<Memory<byte>>();

        public void Debug()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("�ȴ����͵İ�����" + send_queue.Count);
            sb.AppendLine("�Ѿ��յ��İ�����" + recv_queue.Count);

            Console.WriteLine(sb.ToString());
        }

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
            
        }

        int num;
        public void Send(Memory<byte> data)
        {
            num++;
            //Console.WriteLine("Send>>" + num);
            this.send_queue.Enqueue(data);
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
                    if (this._kcpKit.TryRcv(seg.Array, 0, seg.Count))
                    {
                        this.recv_queue.TryDequeue(out _);
                        this._kcpKit.MarkTime();
                    }
                    else
                    {
                        break;
                    }
                }

                while (this.send_queue.Count > 0)
                {
                    this.send_queue.TryPeek(out var item);
                    var seg = item.GetBinaryArray();

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    if (this._kcpKit.TrySnd(seg.Array, 0, seg.Count))
                    {
                        this.send_queue.TryDequeue(out _);
                        this._kcpKit.MarkTime();
                    }
                    else
                    {
                        break;
                    }

                    //Console.WriteLine(sb.ToString());
                }

                this._kcpKit.Update();
            }
            
        }
    }
}