using System;
using System.Buffers;

namespace System.Net
{
    ref struct KcpValueSegment
    {
        public UInt32 conv;
        public byte cmd;
        public byte frg;
        public UInt16 wnd;
        public UInt32 ts;
        public UInt32 sn;
        public UInt32 una;
        public UInt32 length;

        public bool littleEndian;

        internal int encode(byte[] ptr, int offset)
        {
            var offset_ = offset;
            Span<byte> buffer = new Span<byte>(ptr);

            switch (this.littleEndian)
            {
                case true:
                    offset += KcpHelper.WriteUInt32_LE(buffer.Slice(offset), conv);
                    offset += KcpHelper.WriteUInt8(buffer.Slice(offset), cmd);
                    offset += KcpHelper.WriteUInt8(buffer.Slice(offset), frg);
                    offset += KcpHelper.WriteUInt16_LE(buffer.Slice(offset), wnd);
                    offset += KcpHelper.WriteUInt32_LE(buffer.Slice(offset), ts);
                    offset += KcpHelper.WriteUInt32_LE(buffer.Slice(offset), sn);
                    offset += KcpHelper.WriteUInt32_LE(buffer.Slice(offset), una);
                    offset += KcpHelper.WriteUInt32_LE(buffer.Slice(offset), length);
                    break;
                case false:
                    offset += KcpHelper.WriteUInt32_BE(buffer.Slice(offset), conv);
                    offset += KcpHelper.WriteUInt8(buffer.Slice(offset), cmd);
                    offset += KcpHelper.WriteUInt8(buffer.Slice(offset), frg);
                    offset += KcpHelper.WriteUInt16_BE(buffer.Slice(offset), wnd);
                    offset += KcpHelper.WriteUInt32_BE(buffer.Slice(offset), ts);
                    offset += KcpHelper.WriteUInt32_BE(buffer.Slice(offset), sn);
                    offset += KcpHelper.WriteUInt32_BE(buffer.Slice(offset), una);
                    offset += KcpHelper.WriteUInt32_BE(buffer.Slice(offset), length);
                    break;
            }

            return offset - offset_;
        }

        internal bool decode(uint conv, byte[] data, ref int offset)
        {
            var buffer = new Span<byte>(data);
            if (littleEndian)
            {
                offset += KcpHelper.ReadUInt32_LE(buffer.Slice(offset), ref conv);

                if (conv != this.conv) return false;

                offset += KcpHelper.ReadUInt8(buffer.Slice(offset), ref cmd);
                offset += KcpHelper.ReadUInt8(buffer.Slice(offset), ref frg);
                offset += KcpHelper.ReadUInt16_LE(buffer.Slice(offset), ref wnd);
                offset += KcpHelper.ReadUInt32_LE(buffer.Slice(offset), ref ts);
                offset += KcpHelper.ReadUInt32_LE(buffer.Slice(offset), ref sn);
                offset += KcpHelper.ReadUInt32_LE(buffer.Slice(offset), ref una);
                offset += KcpHelper.ReadUInt32_LE(buffer.Slice(offset), ref length);
            }
            else
            {
                offset += KcpHelper.ReadUInt32_BE(buffer.Slice(offset), ref conv);

                if (conv != this.conv) return false;

                offset += KcpHelper.ReadUInt8(buffer.Slice(offset), ref cmd);
                offset += KcpHelper.ReadUInt8(buffer.Slice(offset), ref frg);
                offset += KcpHelper.ReadUInt16_BE(buffer.Slice(offset), ref wnd);
                offset += KcpHelper.ReadUInt32_BE(buffer.Slice(offset), ref ts);
                offset += KcpHelper.ReadUInt32_BE(buffer.Slice(offset), ref sn);
                offset += KcpHelper.ReadUInt32_BE(buffer.Slice(offset), ref una);
                offset += KcpHelper.ReadUInt32_BE(buffer.Slice(offset), ref length);
            }

            return true;
        }
    }
}
