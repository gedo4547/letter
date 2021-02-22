using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Net;

namespace kcp_lab_test
{
    public class KcpUnit
    {
        public KcpUnit(uint conv, MemoryPool<byte> memoryPool)
        {
            this.Conv = conv;
            this._kcpKit = new KcpKit(conv, true, memoryPool);
            this._kcpKit.SettingNoDelay(1, 10, 2, 1);
            this._kcpKit.WriteDelay = false;
            this._kcpKit.SettingStreamMode(true);
        }

        private KcpKit _kcpKit;

        private ConcurrentQueue<Memory<byte>> recv_queue = new ConcurrentQueue<Memory<byte>>();
        private ConcurrentQueue<Memory<byte>> send_queue = new ConcurrentQueue<Memory<byte>>();

        public uint Conv { get; private set; }

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