using System;

namespace Letter.Kcp.lib___
{
    struct KcpValueSegment
    {
        internal UInt32 conv;
        internal UInt32 cmd;
        internal UInt32 frg;
        internal UInt32 wnd;
        internal UInt32 ts;
        internal UInt32 sn;
        internal UInt32 una;

        internal bool littleEndian;

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

        internal bool decode(uint conv, byte[] data, ref int offset)
        {
              var buffer = new Span<byte>(data);
                if(littleEndian)
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
                    offset += KcpHelper.ReadUInt32_BE(buffer.Slice(offset), ref conv_);

                    if (conv != conv_) return -1;

                    offset += KcpHelper.ReadUInt8(buffer.Slice(offset), ref cmd);
                    offset += KcpHelper.ReadUInt8(buffer.Slice(offset), ref frg);
                    offset += KcpHelper.ReadUInt16_BE(buffer.Slice(offset), ref wnd);
                    offset += KcpHelper.ReadUInt32_BE(buffer.Slice(offset), ref ts);
                    offset += KcpHelper.ReadUInt32_BE(buffer.Slice(offset), ref sn);
                    offset += KcpHelper.ReadUInt32_BE(buffer.Slice(offset), ref una);
                    offset += KcpHelper.ReadUInt32_BE(buffer.Slice(offset), ref length);
                }
        }
    }
}
