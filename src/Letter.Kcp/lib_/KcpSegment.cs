using System;
using System.Buffers;
using System.Collections.Generic;

namespace KcpProject
{
    // KCP Segment Definition
    class KcpSegment
    {
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

        private bool isLittleEndian;

        private static Stack<KcpSegment> msSegmentPool = new Stack<KcpSegment>(32);

        public static KcpSegment Get(int size, bool isLittleEndian, KcpBuffer buffer)
        {
            return new KcpSegment(size, isLittleEndian, buffer);
        }

        public static KcpSegment Get(int size, bool isLittleEndian)
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
            return new KcpSegment(size, isLittleEndian);
        }

        public static void Put(KcpSegment seg)
        {
            seg.reset();
            lock (msSegmentPool)
            {
                msSegmentPool.Push(seg);
            }
        }


        private KcpSegment(int size, bool isLittleEndian, KcpBuffer buffer)
        {
            this.isLittleEndian = isLittleEndian;
            this.data = buffer;
        }

        private KcpSegment(int size, bool isLittleEndian)
        {
            this.isLittleEndian = isLittleEndian;
            data = KcpBuffer.Allocate(size, true);
        }

        // encode a segment into buffer
        internal int encode(byte[] ptr, int offset)
        {

            var offset_ = offset;
            // gyd by 2021/1/21/16/51
            if (this.isLittleEndian)
            {
                offset += KcpHelper.encode32u_le(ptr, offset, conv);
                offset += KcpHelper.ikcp_encode8u(ptr, offset, (byte)cmd);
                offset += KcpHelper.ikcp_encode8u(ptr, offset, (byte)frg);
                offset += KcpHelper.encode16u_le(ptr, offset, (UInt16)wnd);
                offset += KcpHelper.encode32u_le(ptr, offset, ts);
                offset += KcpHelper.encode32u_le(ptr, offset, sn);
                offset += KcpHelper.encode32u_le(ptr, offset, una);
                offset += KcpHelper.encode32u_le(ptr, offset, (UInt32)data.ReadableBytes);
            }
            else
            {
                offset += KcpHelper.encode32u_be(ptr, offset, conv);
                offset += KcpHelper.ikcp_encode8u(ptr, offset, (byte)cmd);
                offset += KcpHelper.ikcp_encode8u(ptr, offset, (byte)frg);
                offset += KcpHelper.encode16u_be(ptr, offset, (UInt16)wnd);
                offset += KcpHelper.encode32u_be(ptr, offset, ts);
                offset += KcpHelper.encode32u_be(ptr, offset, sn);
                offset += KcpHelper.encode32u_be(ptr, offset, una);
                offset += KcpHelper.encode32u_be(ptr, offset, (UInt32)data.ReadableBytes);
            }

            // offset += KcpHelper.ikcp_encode32u(ptr, offset, conv);
            // offset += KcpHelper.ikcp_encode8u(ptr, offset, (byte)cmd);
            // offset += KcpHelper.ikcp_encode8u(ptr, offset, (byte)frg);
            // offset += KcpHelper.ikcp_encode16u(ptr, offset, (UInt16)wnd);
            // offset += KcpHelper.ikcp_encode32u(ptr, offset, ts);
            // offset += KcpHelper.ikcp_encode32u(ptr, offset, sn);
            // offset += KcpHelper.ikcp_encode32u(ptr, offset, una);
            // offset += KcpHelper.ikcp_encode32u(ptr, offset, (UInt32)data.ReadableBytes);

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
