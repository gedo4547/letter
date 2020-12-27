using System;
using System.Runtime.CompilerServices;

namespace Letter.Kcp
{
    static class TimeHelpr
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime GetNowTime() => DateTime.UtcNow;
    }
}