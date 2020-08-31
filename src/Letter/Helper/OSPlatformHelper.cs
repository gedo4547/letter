using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Letter.IO
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class OSPlatformHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOSX()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLinux()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }
    }
}
