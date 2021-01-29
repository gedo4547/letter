using System.Net.Sockets.Kcp;

namespace Letter.Kcp
{
    sealed class KcpImpl : Kcp<KcpSegment>
    {
        public KcpImpl(uint conv_, IKcpCallback callback, IRentable rentable) : base(conv_, callback, rentable)
        {
        }

        public uint CurrentConv
        {
            get { return base.conv; }
        }
    }
}
