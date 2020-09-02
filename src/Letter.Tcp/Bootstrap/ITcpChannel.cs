using System.Buffers;
using System.Net;

namespace Letter.Tcp
{
    public interface ITcpChannel : IChannel<ITcpSession>
    {
        // void OnTransportRead(ITcpSession session, EndPoint remotePoint, ref WrappedStreamReader reader);
        // void OnTransportWrite(ITcpSession session, EndPoint remotePoint, ref WrappedStreamWriter writer, ref ReadOnlySequence<byte> sequence);
    }
}