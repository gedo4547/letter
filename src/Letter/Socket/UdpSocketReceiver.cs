using System.IO.Pipelines;

namespace System.Net.Sockets
{
    public class UdpSocketReceiver : SocketBase
    {
        public UdpSocketReceiver(Socket socket, PipeScheduler scheduler) : base(socket, scheduler)
        {
        }
        
        public SocketAwaitableEventArgs ReceiveAsync(EndPoint loacl, Memory<byte> buffer)
        {
#if NETSTANDARD2_0
            ArraySegment<byte> segment = buffer.GetBinaryArray();
            this._awaitableEventArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);
#elif NET5_0
            this._awaitableEventArgs.SetBuffer(buffer);
#endif
            _awaitableEventArgs.RemoteEndPoint = loacl;
            if (!_socket.ReceiveFromAsync(_awaitableEventArgs))
            {
                _awaitableEventArgs.Complete();
            }

            return _awaitableEventArgs;
        }
    }
}