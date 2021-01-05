using System.Net.Sockets.Kcp;

namespace Letter.Kcp
{
    sealed class KcpImpl : System.Net.Sockets.Kcp.Kcp
    {
        public KcpImpl(uint conv_, IKcpCallback callback, IRentable rentable = null)
            : base(conv_, callback, rentable)
        {
        }

        public uint CurrentConv
        {
            get { return base.conv; }
            set { base.conv = value; }
        }
    }
}
