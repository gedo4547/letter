using System;
using System.Buffers.Binary;

namespace KcpProject
{
    static class KcpHelper
    {
        private static DateTime refTime = DateTime.UtcNow;

        public static UInt32 currentMS()
        {
            var ts = DateTime.UtcNow.Subtract(refTime);
            return (UInt32)ts.TotalMilliseconds;
        }

        // encode 8 bits unsigned int
        public static int ikcp_encode8u(byte[] p, int offset, byte c)
        {
            p[0 + offset] = c;
            return 1;
        }

        // decode 8 bits unsigned int
        public static int ikcp_decode8u(byte[] p, int offset, ref byte c)
        {
            c = p[0 + offset];
            return 1;
        }

        public static int encode16u_le(byte[] p, int offset, UInt16 w)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(p.AsSpan(offset), w);
            return 2;
        }

        public static int encode16u_be(byte[] p, int offset, UInt16 w)
        {
            BinaryPrimitives.WriteUInt16BigEndian(p.AsSpan(offset), w);
            return 2;
        }

        public static int decode16u_le(byte[] p, int offset, ref UInt16 c)
        {
            c = BinaryPrimitives.ReadUInt16LittleEndian(p.AsSpan(offset));
            return 2;
        }

        public static int decode16u_be(byte[] p, int offset, ref UInt16 c)
        {
            c = BinaryPrimitives.ReadUInt16BigEndian(p.AsSpan(offset));
            return 2;
        }

        public static int encode32u_le(byte[] p, int offset, UInt32 l)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(p.AsSpan(offset), l);
            return 4;
        }

        public static int encode32u_be(byte[] p, int offset, UInt32 l)
        {
            BinaryPrimitives.WriteUInt32BigEndian(p.AsSpan(offset), l);
            return 4;
        }

        public static int decode32u_le(byte[] p, int offset, ref UInt32 c)
        {
            c = BinaryPrimitives.ReadUInt32LittleEndian(p.AsSpan(offset));
            return 4;
        }

        public static int decode32u_be(byte[] p, int offset, ref UInt32 c)
        {
            c = BinaryPrimitives.ReadUInt32BigEndian(p.AsSpan(offset));
            return 4;
        }

        public static UInt32 _imin_(UInt32 a, UInt32 b)
        {
            return a <= b ? a : b;
        }

        public static UInt32 _imax_(UInt32 a, UInt32 b)
        {
            return a >= b ? a : b;
        }

        public static UInt32 _ibound_(UInt32 lower, UInt32 middle, UInt32 upper)
        {
            return _imin_(_imax_(lower, middle), upper);
        }

        public static Int32 _itimediff(UInt32 later, UInt32 earlier)
        {
            return ((Int32)(later - earlier));
        }
    }
}
