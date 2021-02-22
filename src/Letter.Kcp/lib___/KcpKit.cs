using System;
using Letter.Kcp.lib___;

namespace Letter.IO.Kcplib
{
    public delegate void RefAction(ref Memory<byte> memory);

    public class KcpKit : IDisposable
    {
        
        public KcpKit(uint conv)
        {
            this.mKCP = new KCP(conv, this.OnOutEvent);

            this.mRecvBuffer = ByteBuffer.Allocate(1024 * 32);
            this.mRecvBuffer.Clear();
        }
        
        private KCP mKCP;
        private UInt32 mNextUpdateTime = 0;
        private ByteBuffer mRecvBuffer;

        public RefAction onRcv;
        public RefAction onSnd;
        
        public bool WriteDelay { get; set; }
        
        public bool AckNoDelay { get; set; }

        public void MarkTime()
        {
            this.mNextUpdateTime = KcpHelper.currentMS();
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

        public bool TryRcv(byte[] data, int index, int length, bool regular = true)
        {
            var transfer = this.Recv(data, index, length, regular);
            return transfer == 0;
        }

        public bool TrySnd(byte[] data, int index, int length)
        {
            var transfer = this.Send(data, index, length);
            return transfer > 0;
        }

        private int Send(byte[] data, int index, int length)
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

        private int Recv(byte[] data, int index, int length, bool regular = true)
        {
            var inputN = mKCP.Input(data, index, length, regular, AckNoDelay);
            if (inputN < 0) 
            {
                return inputN;
            }
            
            mRecvBuffer.Clear();

            // 读完所有完整的消息
            for (;;) {
                var size = mKCP.PeekSize();
                if (size <= 0) break;

                mRecvBuffer.EnsureWritableBytes(size);

                var n = mKCP.Recv(mRecvBuffer.RawBuffer, mRecvBuffer.WriterIndex, size);
                if (n > 0) mRecvBuffer.WriterIndex += n;
            }

            // 有数据待接收
            if (mRecvBuffer.ReadableBytes > 0) {
                if(this.onRcv != null)
                {
                    var memory = new Memory<byte>(
                        mRecvBuffer.RawBuffer, 
                        mRecvBuffer.ReaderIndex, 
                        mRecvBuffer.ReadableBytes);

                    this.onRcv(ref memory);
                }
                return 0;
            }

            return 0;
        }

        private void OnOutEvent(byte[] buffer, int length)
        {
            if (this.onSnd != null)
            {
                var memory = new Memory<byte>(buffer, 0, length);

                this.onSnd(ref memory);
            }
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