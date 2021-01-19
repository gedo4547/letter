using System.Buffers.Binary;
using Kcplib = System.Net.Sockets.Kcp.Kcp;

namespace Letter.Kcp
{
    public static class KcpHelpr
    {
        private static IBinaryOrderOperators orderOperators = BinaryOrderOperatorsFactory.GetOperators(GetKcpBinaryOrder());

        public static void SettingKcpBinaryOrder(BinaryOrder order)
        {
            Kcplib.IsLittleEndian = (order == BinaryOrder.LittleEndian);
            orderOperators = BinaryOrderOperatorsFactory.GetOperators(order);
        }

        public static BinaryOrder GetKcpBinaryOrder()
        {
            return Kcplib.IsLittleEndian ? BinaryOrder.LittleEndian : BinaryOrder.BigEndian;
        }

        public static IBinaryOrderOperators GetOperators()
        {
            return orderOperators;
        }
    }
}
