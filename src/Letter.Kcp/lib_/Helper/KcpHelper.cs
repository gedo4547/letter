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

        /* encode 16 bits unsigned int (lsb) */
        public static int ikcp_encode16u(byte[] p, int offset, UInt16 w)
        {
            p[0 + offset] = (byte)(w >> 0);
            p[1 + offset] = (byte)(w >> 8);
            return 2;
        }

        /* decode 16 bits unsigned int (lsb) */
        public static int ikcp_decode16u(byte[] p, int offset, ref UInt16 c)
        {
            UInt16 result = 0;
            result |= (UInt16)p[0 + offset];
            result |= (UInt16)(p[1 + offset] << 8);
            c = result;
            return 2;
        }

        /* encode 32 bits unsigned int (lsb) */
        public static int ikcp_encode32u(byte[] p, int offset, UInt32 l)
        {
            p[0 + offset] = (byte)(l >> 0);
            p[1 + offset] = (byte)(l >> 8);
            p[2 + offset] = (byte)(l >> 16);
            p[3 + offset] = (byte)(l >> 24);
            return 4;
        }

        /* decode 32 bits unsigned int (lsb) */
        public static int ikcp_decode32u(byte[] p, int offset, ref UInt32 c)
        {
            UInt32 result = 0;
            result |= (UInt32)p[0 + offset];
            result |= (UInt32)(p[1 + offset] << 8);
            result |= (UInt32)(p[2 + offset] << 16);
            result |= (UInt32)(p[3 + offset] << 24);
            c = result;
            return 4;
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
