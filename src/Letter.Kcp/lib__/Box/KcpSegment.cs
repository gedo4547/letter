using System;
using System.Buffers;

namespace Letter.Kcp.lib__
{
    class KcpSegment : IDisposable
    {
        public KcpSegment(bool useLittleEndian, KcpBuffer buffer)
        {
            this.data = buffer;
            this.useLittleEndian = useLittleEndian;
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
        internal KcpBuffer data;

        private bool useLittleEndian;

        internal int encode(byte[] ptr, int offset)
        {
            var offset_ = offset;
            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(ptr);

            switch (this.useLittleEndian)
            {
                case true:
                    offset += KcpHelper.WriteUInt32_LE(buffer.Slice(offset), conv);
                    offset += KcpHelper.WriteUInt8(buffer.Slice(offset), (byte)cmd);
                    offset += KcpHelper.WriteUInt8(buffer.Slice(offset), (byte)frg);
                    offset += KcpHelper.WriteUInt16_LE(buffer.Slice(offset), (UInt16)wnd);
                    offset += KcpHelper.WriteUInt32_LE(buffer.Slice(offset), ts);
                    offset += KcpHelper.WriteUInt32_LE(buffer.Slice(offset), sn);
                    offset += KcpHelper.WriteUInt32_LE(buffer.Slice(offset), una);
                    offset += KcpHelper.WriteUInt32_LE(buffer.Slice(offset), (UInt32)data.ReadableLength);
                    break;
                case false:
                    offset += KcpHelper.WriteUInt32_BE(buffer.Slice(offset), conv);
                    offset += KcpHelper.WriteUInt8(buffer.Slice(offset), (byte)cmd);
                    offset += KcpHelper.WriteUInt8(buffer.Slice(offset), (byte)frg);
                    offset += KcpHelper.WriteUInt16_BE(buffer.Slice(offset), (UInt16)wnd);
                    offset += KcpHelper.WriteUInt32_BE(buffer.Slice(offset), ts);
                    offset += KcpHelper.WriteUInt32_BE(buffer.Slice(offset), sn);
                    offset += KcpHelper.WriteUInt32_BE(buffer.Slice(offset), una);
                    offset += KcpHelper.WriteUInt32_BE(buffer.Slice(offset), (UInt32)data.ReadableLength);
                    break;
            }

            return offset - offset_;
        }

        internal void Reset()
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
            
            this.data.Reset();
        }

        public void Dispose()
        {
            if (this.data != null)
            {
                this.data = null;
            }
        }
    }
}
