﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Net.Sockets
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SocketErrorHelper
    {
        private static readonly bool IsWindows = OSPlatformHelper.IsWindows();

        public static bool IsSocketDisabledError(Exception ex)
        {
            if (ex is SocketException socketEx)
            {
                return IsConnectionResetError(socketEx.SocketErrorCode) ||
                       IsConnectionAbortError(socketEx.SocketErrorCode);
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSocketDisabledError(SocketError error)
        {
            return IsConnectionResetError(error) || IsConnectionAbortError(error);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsConnectionResetError(SocketError errorCode)
        {
            // A connection reset can be reported as SocketError.ConnectionAborted on Windows.
            // ProtocolType can be removed once https://github.com/dotnet/corefx/issues/31927 is fixed.
            return errorCode == SocketError.ConnectionReset ||
                   errorCode == SocketError.Shutdown ||
                   (errorCode == SocketError.ConnectionAborted && IsWindows);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsConnectionAbortError(SocketError errorCode)
        {
            // Calling Dispose after ReceiveAsync can cause an "InvalidArgument" error on *nix.
            return errorCode == SocketError.OperationAborted ||
                   errorCode == SocketError.Interrupted ||
                   (errorCode == SocketError.InvalidArgument && !IsWindows);
        }
    }
}