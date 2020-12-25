﻿using System.Runtime.CompilerServices;
using Kcptun = System.Net.Sockets.Kcp.Kcp;
using Letter.Kcp;

namespace System
{
    static class KcpExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetNoDelay(this Kcptun kcp, NoDelayConfig config)
        {
            kcp.NoDelay(config.nodelay_, config.interval_, config.resend_, config.nc_);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetWndSize(this Kcptun kcp, WndSizeConfig config)
        {
            kcp.WndSize(config.sndwnd, config.rcvwnd);
        }
    }
}