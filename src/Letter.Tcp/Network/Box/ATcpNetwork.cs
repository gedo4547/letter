using System.Net.Sockets;

namespace Letter.Tcp.Box
{
    abstract class ATcpNetwork<TTcpOptions> : Letter.Box.ANetwork<TTcpOptions>
        where TTcpOptions : ATcpOptions
    {
        public static readonly bool IsMacOS = OSPlatformHelper.IsOSX();
        public static readonly bool IsWindows = OSPlatformHelper.IsWindows();
        
        
        protected ATcpNetwork(TTcpOptions options) : base(options)
        {
            
        }
        
        protected static bool IsConnectionResetError(SocketError errorCode)
        {
            // A connection reset can be reported as SocketError.ConnectionAborted on Windows.
            // ProtocolType can be removed once https://github.com/dotnet/corefx/issues/31927 is fixed.
            return errorCode == SocketError.ConnectionReset ||
                   errorCode == SocketError.Shutdown ||
                   (errorCode == SocketError.ConnectionAborted && IsWindows) ||
                   (errorCode == SocketError.ProtocolType && IsMacOS);
        }

        protected static bool IsConnectionAbortError(SocketError errorCode)
        {
            // Calling Dispose after ReceiveAsync can cause an "InvalidArgument" error on *nix.
            return errorCode == SocketError.OperationAborted ||
                   errorCode == SocketError.Interrupted ||
                   (errorCode == SocketError.InvalidArgument && !IsWindows);
        }
    }
}