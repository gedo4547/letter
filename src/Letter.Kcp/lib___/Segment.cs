using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.IO;

namespace Letter.IO.Kcplib
{
    // KCP Segment Definition
    internal class Segment
    {
        private static readonly RecyclableMemoryStreamManager streamManager = new RecyclableMemoryStreamManager();
        private static Stack<Segment> msSegmentPool = new Stack<Segment>(32);

        private Segment(bool littleEndian)
        {
            this.littleEndian = littleEndian;
            this.stream = streamManager.GetStream();
        }

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
        // internal ByteBuffer data;

        internal MemoryStream stream;
        private bool littleEndian;

        public static Segment Get(bool littleEndian)
        {
            if (msSegmentPool.Count > 0)
            {
                var seg = msSegmentPool.Pop();
                return seg;
            }

            return new Segment(littleEndian);
        }

        public static void Put(Segment seg)
        {
            seg.reset();
            msSegmentPool.Push(seg);
        }

        

        // encode a segment into buffer
        internal int encode(byte[] ptr, int offset)
        {
            var offset_ = offset;
            Span<byte> buffer = new Span<byte>(ptr);

            switch (this.littleEndian)
            {
                case true:
                    offset += KcpHelper.WriteUInt32_LE(buffer.Slice(offset), conv);
                    offset += KcpHelper.WriteUInt8(buffer.Slice(offset), (byte)cmd);
                    offset += KcpHelper.WriteUInt8(buffer.Slice(offset), (byte)frg);
                    offset += KcpHelper.WriteUInt16_LE(buffer.Slice(offset), (ushort)wnd);
                    offset += KcpHelper.WriteUInt32_LE(buffer.Slice(offset), ts);
                    offset += KcpHelper.WriteUInt32_LE(buffer.Slice(offset), sn);
                    offset += KcpHelper.WriteUInt32_LE(buffer.Slice(offset), una);
                    offset += KcpHelper.WriteUInt32_LE(buffer.Slice(offset), (UInt32)this.stream.Position);
                    break;
                case false:
                    offset += KcpHelper.WriteUInt32_BE(buffer.Slice(offset), conv);
                    offset += KcpHelper.WriteUInt8(buffer.Slice(offset), (byte)cmd);
                    offset += KcpHelper.WriteUInt8(buffer.Slice(offset), (byte)frg);
                    offset += KcpHelper.WriteUInt16_BE(buffer.Slice(offset), (ushort)wnd);
                    offset += KcpHelper.WriteUInt32_BE(buffer.Slice(offset), ts);
                    offset += KcpHelper.WriteUInt32_BE(buffer.Slice(offset), sn);
                    offset += KcpHelper.WriteUInt32_BE(buffer.Slice(offset), una);
                    offset += KcpHelper.WriteUInt32_BE(buffer.Slice(offset), (UInt32)this.stream.Position);
                    break;
            }

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

            this.stream.SetLength(0);

            // data.Clear();
            // data.Dispose();
            // data = null;
        }
    }
}
