using System.Net;
using System.Runtime.CompilerServices;

using Letter.Kcp;

namespace System
{
    static class KcpExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SettingNoDelay(this KcpKit kcp, NoDelayConfig config)
        {
            kcp.SettingNoDelay(config.nodelay_, config.interval_, config.resend_, config.nc_);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SettingWndSize(this KcpKit kcp, WndSizeConfig config)
        {
            kcp.SettingWndSize(config.sndwnd, config.rcvwnd);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SettingMtu(this KcpKit kcp, int? mtu)
        {
            if (mtu == null) return;

            kcp.SettingMtu(mtu.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SettingStreamMode(this KcpKit kcp, bool? enabled)
        {
            if (enabled == null) return;

            kcp.SettingStreamMode(enabled.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SettingReservedSize(this KcpKit kcp, int? reservedSize)
        {
            if (reservedSize == null) return;

            kcp.SettingReserveBytes(reservedSize.Value);
        }
    }
}