using System.IO.Pipelines;

namespace System.Net.Sockets
{
    public sealed class TcpSocketReceiver : SocketBase
    {
        public TcpSocketReceiver(Socket socket, PipeScheduler scheduler) : base(socket, scheduler)
        {
        }

        public SocketAwaitableEventArgs WaitForDataAsync()
        {
#if NETSTANDARD2_0
            _awaitableEventArgs.SetBuffer(Array.Empty<byte>(), 0, 0); 
#elif NET5_0
            _awaitableEventArgs.SetBuffer(Memory<byte>.Empty);
#endif
            if (!_socket.ReceiveAsync(_awaitableEventArgs))
            {
                _awaitableEventArgs.Complete();
            }

            return _awaitableEventArgs;
        }

        public SocketAwaitableEventArgs ReceiveAsync(Memory<byte> buffer)
        {
#if NETSTANDARD2_0
            ArraySegment<byte> segment = buffer.GetBinaryArray();
            this._awaitableEventArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);
#elif NET5_0
            this._awaitableEventArgs.SetBuffer(buffer);
#endif

            if (!_socket.ReceiveAsync(_awaitableEventArgs))
            {
                _awaitableEventArgs.Complete();
            }

            return _awaitableEventArgs;
        }
    }
}