using System;
using System.Collections.Generic;

namespace KcpProject
{
    // KCP Segment Definition
    internal class Segment
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
            internal ByteBuffer data;

            private static Stack<Segment> msSegmentPool = new Stack<Segment>(32);

            public static Segment Get(int size)
            {
                lock (msSegmentPool)
                {
                    if (msSegmentPool.Count > 0)
                    {
                        var seg = msSegmentPool.Pop();
                        seg.data = ByteBuffer.Allocate(size, true);
                        return seg;
                    }
                }
                return new Segment(size);
            }

            public static void Put(Segment seg)
            {
                seg.reset();
                lock (msSegmentPool) {
                    msSegmentPool.Push(seg);
                }
            }

            private Segment(int size)
            {
                data = ByteBuffer.Allocate(size, true);
            }

            // encode a segment into buffer
            internal int encode(byte[] ptr, int offset)
            {

                var offset_ = offset;

                offset += KCP.ikcp_encode32u(ptr, offset, conv);
                offset += KCP.ikcp_encode8u(ptr, offset, (byte)cmd);
                offset += KCP.ikcp_encode8u(ptr, offset, (byte)frg);
                offset += KCP.ikcp_encode16u(ptr, offset, (UInt16)wnd);
                offset += KCP.ikcp_encode32u(ptr, offset, ts);
                offset += KCP.ikcp_encode32u(ptr, offset, sn);
                offset += KCP.ikcp_encode32u(ptr, offset, una);
                offset += KCP.ikcp_encode32u(ptr, offset, (UInt32)data.ReadableBytes);

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
