using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class OSPlatformHelper
    {
        private readonly static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private readonly static bool isOSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        private readonly static bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWindows() => isWindows;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOSX() => isOSX;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLinux() => isLinux;
    }
}
