﻿namespace Letter.Tcp
{
    public class SocketsStrings
    {
        internal static string @FileHandleEndPointNotSupported =>
            "The Socket transport does not support binding to file handles. Consider using the libuv transport instead.";

        /// <summary>Only ListenType.IPEndPoint is supported by the Socket Transport. https://go.microsoft.com/fwlink/?linkid=874850</summary>
        internal static string @OnlyIPEndPointsSupported =>
            "Only ListenType.IPEndPoint is supported by the Socket Transport. https://go.microsoft.com/fwlink/?linkid=874850";

        /// <summary>Transport is already bound.</summary>
        internal static string @TransportAlreadyBound =>
            "Transport is already bound.";
    }
}