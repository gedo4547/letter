using System;
using System.Buffers;

namespace Letter.Kcp.lib__
{
    public delegate void RefAction(ref ReadOnlySequence<byte> sequence);

    public sealed class KcpKit : IDisposable
    {
        public KcpKit(uint conv, bool littleEndian, MemoryPool<byte> memoryPool)
        {
            this.mKCP = new Kcp(conv, littleEndian, memoryPool, this.OnOutEvent);

            this._allotter = new KcpMemoryBlockAllotter(memoryPool);
            this.buffer = new KcpBuffer(this._allotter);
        }
        
        private Kcp mKCP;
        private UInt32 mNextUpdateTime = 0;

        private KcpBuffer buffer;
        private KcpMemoryBlockAllotter _allotter;

        public RefAction onRcv;
        public RefAction onSnd;

        public bool WriteDelay { get; set; } = false;
        
        public bool AckNoDelay { get; set; }

        public void SetNextTime(uint time)
        {
            this.mNextUpdateTime = time;
        }

        public void SettingNoDelay(int nodelay_, int interval_, int resend_, int nc_)
        {
            this.mKCP.NoDelay(nodelay_, interval_, resend_, nc_);
        }

        public void SettingStreamMode(bool enabled)
        {
            this.mKCP.SetStreamMode(enabled);
        }

        public void SettingWndSize(int sndwnd, int rcvwnd)
        {
            this.mKCP.WndSize(sndwnd, rcvwnd);
        }

        public bool SettingMtu(int mtu_)
        {
            return this.mKCP.SetMtu(mtu_) == 0;
        }

        public bool SettingReserveBytes(int reservedSize)
        {
            return this.mKCP.ReserveBytes(reservedSize);
        }
        
        public int Send(byte[] data, int index, int length)
        {
            var waitsnd = mKCP.WaitSnd;
            if (waitsnd < mKCP.SndWnd && waitsnd < mKCP.RmtWnd) {

                var sendBytes = 0;
                do {
                    var n = Math.Min((int)mKCP.Mss, length - sendBytes);
                    mKCP.Send(data, index + sendBytes, n);
                    sendBytes += n;
                } while (sendBytes < length);

                waitsnd = mKCP.WaitSnd;
                if (waitsnd >= mKCP.SndWnd || waitsnd >= mKCP.RmtWnd || !WriteDelay) {
                    mKCP.Flush(false);
                }

                return length;
            }

            return 0;
        }

        /// <summary>
        /// 接收
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <param name="regular">普通包(非向前纠正的包)</param>
        /// <returns></returns>
        public int Recv(byte[] data, int index, int length, bool regular = true)
        {
            var inputN = mKCP.Input(data, index, length, regular, AckNoDelay);
            if (inputN < 0) 
            {
                return inputN;
            }
            
            this.buffer.Reset();

            // 读完所有完整的消息
            for (;;) {
                var size = mKCP.PeekSize();
                if (size <= 0) break;

                mKCP.Recv(this.buffer);
            }

            // 有数据待接收
            var readableBuffer = this.buffer.ReadableBuffer;
            if (readableBuffer.Length > 0)
            {
                if (this.onRcv != null)
                {
                    this.onRcv(ref readableBuffer);
                }
            }

            return 0;
        }

        private void OnOutEvent(byte[] buffer, int length)
        {
            //Console.WriteLine("kcp send>>>" + length + ">>>>>>>>>>>>>>" + (this.onSnd == null));
            if (this.onSnd == null)
            {
                return;
            }

            var sequence = new ReadOnlySequence<byte>(buffer, 0, length);
            this.onSnd(ref sequence);
        }

        public void Update()
        {
            if (0 == mNextUpdateTime || mKCP.CurrentMS >= mNextUpdateTime)
            {
                mKCP.Update();
                mNextUpdateTime = mKCP.Check();
            }
        }

        public void Dispose()
        {
            
        }
    }
}