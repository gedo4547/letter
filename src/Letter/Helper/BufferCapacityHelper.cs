namespace Letter
{
    static class BufferCapacityHelper
    {
        public static int GetSuitableBufferSize(int size)
        {
            const int basePoint = 64;
            if (size <= basePoint)
            {
                return basePoint;
            }

            int remainder = size % basePoint;

            if(remainder == 0)
            {
                return size;
            }

            return size + basePoint - remainder;
        }
    }
}
