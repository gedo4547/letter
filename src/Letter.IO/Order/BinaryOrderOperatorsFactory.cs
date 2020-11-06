namespace System.Buffers.Binary
{
    static class BinaryOrderOperatorsFactory
    {
        private static IBinaryOrderOperators bigEndian = new OrderOperatorsBigEndianImpl();
        private static IBinaryOrderOperators littleEndian = new OrderOperatorsLittleEndianImpl();
        
        public static IBinaryOrderOperators GetOperators(BinaryOrder order)
        {
            switch (order)
            {
                case BinaryOrder.BigEndian: return bigEndian;
                case BinaryOrder.LittleEndian: return littleEndian;
            }

            return null;
        }
    }
}