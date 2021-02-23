using System;
using System.Buffers;
using System.Collections.Concurrent;
using Letter.IO.Kcplib;

namespace kcp_lab_test2
{
    public class KcpUnit
    {
        public KcpUnit(uint conv)
        {
            this.Conv = conv;

            this._kcpKit = new KcpKit(conv);
            this._kcpKit.SettingNoDelay(1, 10, 2, 1);
            this._kcpKit.WriteDelay = false;
            this._kcpKit.SettingStreamMode(true);


        }

        private KcpKit _kcpKit;

        private ConcurrentQueue<Memory<byte>> recv_queue = new ConcurrentQueue<Memory<byte>>();
        private ConcurrentQueue<Memory<byte>> send_queue = new ConcurrentQueue<Memory<byte>>();

        public uint Conv
        {
            get;
            private set;
        }

        public void Debug()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("等待发送的包：：" + send_queue.Count);
            sb.AppendLine("已经收到的包：：" + recv_queue.Count);

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