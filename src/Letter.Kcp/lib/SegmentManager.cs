using System.Net.Sockets.Kcp;

namespace Letter.Kcp
{
    sealed class SegmentManager : ISegmentManager<KcpSegment>
    {
        public KcpSegment Alloc(int appendDateSize)
        {
            throw new System.NotImplementedException();
        }

        public void Free(KcpSegment seg)
        {
            throw new System.NotImplementedException();
        }
    }
}