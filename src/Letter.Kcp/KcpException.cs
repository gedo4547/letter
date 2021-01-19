using System;

namespace Letter.Kcp
{
    public sealed class KcpException : Exception
    {
        public KcpException(int errorCode) : base($"KCP parsing failed, error code：{errorCode}")
        {

        }
    }
}
