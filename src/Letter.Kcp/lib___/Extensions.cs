using System;
using System.IO;

namespace Letter.IO.Kcplib
{
    public static class Extensions
    {
        public static int ReaderIndex(this MemoryStream stream)
        {
            return 0;
        }

        public static int ReadableBytes(this MemoryStream stream)
        {
            return (int)stream.Position;
        }
    }
}
