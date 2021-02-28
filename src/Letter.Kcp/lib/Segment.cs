using System.Runtime.CompilerServices;

namespace System.Net
{
    class Segment : IDisposable
    {
        public Segment(bool littleEndian_, Buffer buffer)
        {
            this.data = buffer;
            this.feature = new Feature()
            {
                littleEndian = littleEndian_
            };
        }

        private Feature feature;
        
        public UInt32 rto = 0;
        public UInt32 xmit = 0;
        public UInt32 resendts = 0;
        public UInt32 fastack = 0;
        public UInt32 acked = 0;
        
        public Buffer data;

        public UInt32 conv
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.feature.conv; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { this.feature.conv = value; }
        }

        public byte cmd
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.feature.cmd; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { this.feature.cmd = value; }
        }

        public byte frg
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.feature.frg; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { this.feature.frg = value; }
        }

        public ushort wnd
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.feature.wnd; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { this.feature.wnd = value; }
        }

        public UInt32 ts
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.feature.ts; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { this.feature.ts = value; }
        }

        public UInt32 sn
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.feature.sn; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { this.feature.sn = value; }
        }

        public UInt32 una
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.feature.una; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { this.feature.una = value; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int encode(byte[] ptr, int offset)
        {
            this.feature.length = (uint)data.ReadableLength;
            return this.feature.encode(ptr, offset);
        }

        public void Reset()
        {
            this.rto = 0;
            this.xmit = 0;
            this.resendts = 0;
            this.fastack = 0;
            this.acked = 0;
            
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
