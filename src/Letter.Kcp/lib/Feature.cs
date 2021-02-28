namespace System.Net
{
    struct Feature
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
                    offset += Helper.WriteUInt32_LE(buffer.Slice(offset), conv);
                    offset += Helper.WriteUInt8(buffer.Slice(offset), cmd);
                    offset += Helper.WriteUInt8(buffer.Slice(offset), frg);
                    offset += Helper.WriteUInt16_LE(buffer.Slice(offset), wnd);
                    offset += Helper.WriteUInt32_LE(buffer.Slice(offset), ts);
                    offset += Helper.WriteUInt32_LE(buffer.Slice(offset), sn);
                    offset += Helper.WriteUInt32_LE(buffer.Slice(offset), una);
                    offset += Helper.WriteUInt32_LE(buffer.Slice(offset), length);
                    break;
                case false:
                    offset += Helper.WriteUInt32_BE(buffer.Slice(offset), conv);
                    offset += Helper.WriteUInt8(buffer.Slice(offset), cmd);
                    offset += Helper.WriteUInt8(buffer.Slice(offset), frg);
                    offset += Helper.WriteUInt16_BE(buffer.Slice(offset), wnd);
                    offset += Helper.WriteUInt32_BE(buffer.Slice(offset), ts);
                    offset += Helper.WriteUInt32_BE(buffer.Slice(offset), sn);
                    offset += Helper.WriteUInt32_BE(buffer.Slice(offset), una);
                    offset += Helper.WriteUInt32_BE(buffer.Slice(offset), length);
                    break;
            }

            return offset - offset_;
        }

        internal bool decode(uint currentConv, byte[] data, ref int offset)
        {
            var buffer = new Span<byte>(data);
            if (littleEndian)
            {
                offset += Helper.ReadUInt32_LE(buffer.Slice(offset), ref conv);

                if (currentConv != this.conv) return false;

                offset += Helper.ReadUInt8(buffer.Slice(offset), ref cmd);
                offset += Helper.ReadUInt8(buffer.Slice(offset), ref frg);
                offset += Helper.ReadUInt16_LE(buffer.Slice(offset), ref wnd);
                offset += Helper.ReadUInt32_LE(buffer.Slice(offset), ref ts);
                offset += Helper.ReadUInt32_LE(buffer.Slice(offset), ref sn);
                offset += Helper.ReadUInt32_LE(buffer.Slice(offset), ref una);
                offset += Helper.ReadUInt32_LE(buffer.Slice(offset), ref length);
            }
            else
            {
                offset += Helper.ReadUInt32_BE(buffer.Slice(offset), ref conv);

                if (currentConv != this.conv) return false;

                offset += Helper.ReadUInt8(buffer.Slice(offset), ref cmd);
                offset += Helper.ReadUInt8(buffer.Slice(offset), ref frg);
                offset += Helper.ReadUInt16_BE(buffer.Slice(offset), ref wnd);
                offset += Helper.ReadUInt32_BE(buffer.Slice(offset), ref ts);
                offset += Helper.ReadUInt32_BE(buffer.Slice(offset), ref sn);
                offset += Helper.ReadUInt32_BE(buffer.Slice(offset), ref una);
                offset += Helper.ReadUInt32_BE(buffer.Slice(offset), ref length);
            }

            return true;
        }

        public void Reset()
        {
            this.conv = 0;
            this.cmd = 0;
            this.frg = 0;
            this.wnd = 0;
            this.ts = 0;
            this.sn = 0;
            this.una = 0;
            this.length = 0;
        }

    }
}
