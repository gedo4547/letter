using System.Collections.Generic;

namespace System.Net.Sockets
{
    // KCP Segment Definition
    class KcpSegment 
    {
        public KcpSegment(bool useLittleEndian, KcpBuffer buffer)
        {
            this.data = buffer;
            this.useLittleEndian = useLittleEndian;
        }

        private bool useLittleEndian;
        
        internal UInt32 conv = 0;
        internal UInt32 cmd = 0;
        internal UInt32 frg = 0;
        internal UInt32 wnd = 0;
        internal UInt32 ts = 0;
        internal UInt32 sn = 0;
        internal UInt32 una = 0;
        internal UInt32 rto = 0;
        internal UInt32 xmit = 0;
        internal UInt32 resendts = 0;
        internal UInt32 fastack = 0;
        internal UInt32 acked = 0;
        internal KcpBuffer data;
        
        

        private static Stack<KcpSegment> msSegmentPool = new Stack<KcpSegment>(32);

        public static KcpSegment Get(int size)
        {
            lock (msSegmentPool)
            {
                if (msSegmentPool.Count > 0)
                {
                    var seg = msSegmentPool.Pop();
                    seg.data = KcpBuffer.Allocate(size, true);
                    return seg;
                }
            }
            return new KcpSegment(size);
        }

        public static void Put(KcpSegment seg)
        {
            seg.reset();
            lock (msSegmentPool) {
                msSegmentPool.Push(seg);
            }
        }

        private KcpSegment(int size)
        {
            data = KcpBuffer.Allocate(size, true);
        }

        // encode a segment into buffer
        internal int encode(byte[] ptr, int offset)
        {

            var offset_ = offset;

            // offset += ikcp_encode32u(ptr, offset, conv);
            // offset += ikcp_encode8u(ptr, offset, (byte)cmd);
            // offset += ikcp_encode8u(ptr, offset, (byte)frg);
            // offset += ikcp_encode16u(ptr, offset, (UInt16)wnd);
            // offset += ikcp_encode32u(ptr, offset, ts);
            // offset += ikcp_encode32u(ptr, offset, sn);
            // offset += ikcp_encode32u(ptr, offset, una);
            // offset += ikcp_encode32u(ptr, offset, (UInt32)data.ReadableBytes);

            return offset - offset_;
        }

        internal void reset()
        {
            conv = 0;
            cmd = 0;
            frg = 0;
            wnd = 0;
            ts = 0;
            sn = 0;
            una = 0;
            rto = 0;
            xmit = 0;
            resendts = 0;
            fastack = 0;
            acked = 0;

            data.Clear();
            data.Dispose();
            data = null;
        }
    }
}