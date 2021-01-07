using System.Buffers.Binary;
using Kcplib = System.Net.Sockets.Kcp.Kcp;

namespace Letter.Kcp
{
    public static class KcpHelpr
    {
        public static BinaryOrder KcpGlobalBinaryOrder
        {
            get
            {
                return Kcplib.IsLittleEndian ? BinaryOrder.LittleEndian : BinaryOrder.BigEndian;
            }
            set
            {
                Kcplib.IsLittleEndian = (value == BinaryOrder.LittleEndian);
            }
        }
    }
}
