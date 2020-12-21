using System;
using System.IO.Pipelines;
using Letter.IO;
using Letter.Udp;

namespace Letter.Kcp
{
    public interface IChannelController : IDisposable
    {
        bool RegisterSession(IKcpSession session);
        bool UnregisterSession(IKcpSession session);
        void OnRcvUdpMessage(IUdpSession session, ref WrappedReader reader, WrappedArgs args);
        void OnSndUdpMessage(IUdpSession session, ref WrappedWriter writer, WrappedArgs args);
    }
}