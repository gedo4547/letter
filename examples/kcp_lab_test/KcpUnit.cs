using System.Buffers;
using Letter.Kcp.lib__;

namespace kcp_lab_test
{
    public class KcpUnit
    {
        public KcpUnit(MemoryPool<byte> memoryPool)
        {
            this._kcpKit = new KcpKit(1, true, memoryPool);
            this._kcpKit.SettingNoDelay(1, 10, 2, 1);
            this._kcpKit.SettingStreamMode(false);
        }

        private KcpKit _kcpKit;

        public void SetRcvEvent(RefAction rcv)
        {
            this._kcpKit.onRcv += rcv;
        }

        public void SetSndEvent(RefAction snd)
        {
            this._kcpKit.onSnd += snd;
        }

        public int Recv(byte[] data, int index, int length, bool regular = true)
        {
            return this._kcpKit.Recv(data, index, length, regular);
        }

        public int Send(byte[] data, int index, int length)
        {
            return this._kcpKit.Send(data, index, length);
        }

        public void Update()
        {
            this._kcpKit.Update();
        }
    }
}