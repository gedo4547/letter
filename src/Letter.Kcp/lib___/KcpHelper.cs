using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Letter.Kcp.lib___
{
    static class KcpHelper
    {
        private static DateTime refTime = DateTime.Now;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 currentMS()
        {
            var ts = DateTime.Now.Subtract(refTime);
            return (UInt32)ts.TotalMilliseconds;
        }
    }
}
