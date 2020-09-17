using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.InteropServices;

namespace System.Net.Sockets
{
    public class UdpSocketSender : SocketSender
    {
        public UdpSocketSender(Socket socket, PipeScheduler scheduler) : base(socket, scheduler)
        {
        }
        
        public SocketAwaitableEventArgs SendAsync(EndPoint remotePoint, in ReadOnlySequence<byte> buffers)
        {
            if (buffers.IsSingleSegment)
            {
                return SendAsync(remotePoint, buffers.First);
            }

            if (!_awaitableEventArgs.MemoryBuffer.Equals(Memory<byte>.Empty))
            {
                _awaitableEventArgs.SetBuffer(null, 0, 0);
            }

            _awaitableEventArgs.BufferList = GetBufferList(buffers);
            _awaitableEventArgs.RemoteEndPoint = remotePoint;
            if (!_socket.SendToAsync(_awaitableEventArgs))
            {
                _awaitableEventArgs.Complete();
            }

            return _awaitableEventArgs;
        }

        private SocketAwaitableEventArgs SendAsync(EndPoint remotePoint, ReadOnlyMemory<byte> memory)
        {
            // The BufferList getter is much less expensive then the setter.
            if (_awaitableEventArgs.BufferList != null)
            {
                _awaitableEventArgs.BufferList = null;
            }

            _awaitableEventArgs.SetBuffer(MemoryMarshal.AsMemory(memory));
            _awaitableEventArgs.RemoteEndPoint = remotePoint;
            if (!_socket.SendToAsync(_awaitableEventArgs))
            {
                _awaitableEventArgs.Complete();
            }

            return _awaitableEventArgs;
        }
    }
}