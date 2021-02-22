using System;
using System.Buffers;

namespace System.Net
{
    ref struct KcpValueSegment
    {
        public KcpValueSegment(bool littleEndian)
        {
            this.conv = 0;
            this.cmd = 0;
            this.frg = 0;
            this.wnd = 0;
            this.ts = 0;
            this.sn = 0;
            this.una = 0;
            this.rto = 0;
            this.xmit = 0;
            this.resendts = 0;
            this.fastack = 0;
            this.acked = 0;

            //this.data = data;
            this.littleEndian = littleEndian;
        }

        internal UInt32 conv;
        internal UInt32 cmd;
        internal UInt32 frg;
        internal UInt32 wnd;
        internal UInt32 ts;
        internal UInt32 sn;
        internal UInt32 una;
        internal UInt32 rto;
        internal UInt32 xmit;
        internal UInt32 resendts;
        internal UInt32 fastack;
        internal UInt32 acked;

        //private readonly Span<byte> data;
        private readonly bool littleEndian;

        internal int encode(byte[] ptr, int offset)
        {
            var offset_ = offset;
            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(ptr);

            switch (this.littleEndian)
            {
                case true:
                    offset += KcpHelper.WriteUInt32_LE(buffer.Slice(offset), conv);
                    offset += KcpHelper.WriteUInt8(buffer.Slice(offset), (byte)cmd);
                    offset += KcpHelper.WriteUInt8(buffer.Slice(offset), (byte)frg);
                    offset += KcpHelper.WriteUInt16_LE(buffer.Slice(offset), (UInt16)wnd);
                    offset += KcpHelper.WriteUInt32_LE(buffer.Slice(offset), ts);
                    offset += KcpHelper.WriteUInt32_LE(buffer.Slice(offset), sn);
                    offset += KcpHelper.WriteUInt32_LE(buffer.Slice(offset), una);
                    offset += KcpHelper.WriteUInt32_LE(buffer.Slice(offset), (UInt32)0);
                    break;
                case false:
                    offset += KcpHelper.WriteUInt32_BE(buffer.Slice(offset), conv);
                    offset += KcpHelper.WriteUInt8(buffer.Slice(offset), (byte)cmd);
                    offset += KcpHelper.WriteUInt8(buffer.Slice(offset), (byte)frg);
                    offset += KcpHelper.WriteUInt16_BE(buffer.Slice(offset), (UInt16)wnd);
                    offset += KcpHelper.WriteUInt32_BE(buffer.Slice(offset), ts);
                    offset += KcpHelper.WriteUInt32_BE(buffer.Slice(offset), sn);
                    offset += KcpHelper.WriteUInt32_BE(buffer.Slice(offset), una);
                    offset += KcpHelper.WriteUInt32_BE(buffer.Slice(offset), (UInt32)0);
                    break;
            }

            return offset - offset_;
        }
    }
}
