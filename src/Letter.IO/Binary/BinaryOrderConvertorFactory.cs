namespace Letter.IO
{
    public static class BinaryOrderConvertorFactory
    {
        private static IBinaryOrderConvertor bigEndian = new OrderConvertorBigEndianImpl();
        private static IBinaryOrderConvertor littleEndian = new OrderConvertorLittleEndianImpl();
        
        public static IBinaryOrderConvertor GetConvertor(BinaryOrder order)
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