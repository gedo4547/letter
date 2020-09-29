using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.InteropServices;

namespace System.Net.Sockets
{
    public sealed class TcpSocketSender : SocketSender
    {
        public TcpSocketSender(Socket socket, PipeScheduler scheduler) : base(socket, scheduler)
        {
        }

        public SocketAwaitableEventArgs SendAsync(in ReadOnlySequence<byte> buffers)
        {
            if (buffers.IsSingleSegment)
            {
                return SendAsync(buffers.First);
            }

#if NETSTANDARD2_0
            if (!Array.Empty<byte>().Equals(this._awaitableEventArgs.Buffer))
            {
                this._awaitableEventArgs.SetBuffer(null, 0, 0);
            }
#elif NET5_0
            if (!_awaitableEventArgs.MemoryBuffer.Equals(Memory<byte>.Empty))
            {
                _awaitableEventArgs.SetBuffer(null, 0, 0);
            }
#endif

            _awaitableEventArgs.BufferList = GetBufferList(buffers);

            if (!_socket.SendAsync(_awaitableEventArgs))
            {
                _awaitableEventArgs.Complete();
            }

            return _awaitableEventArgs;
        }

        private SocketAwaitableEventArgs SendAsync(ReadOnlyMemory<byte> memory)
        {
            // The BufferList getter is much less expensive then the setter.
            if (_awaitableEventArgs.BufferList != null)
            {
                _awaitableEventArgs.BufferList = null;
            }

#if NETSTANDARD2_0
            var segment = memory.GetBinaryArray();
            this._awaitableEventArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);
#elif NET5_0
            this._awaitableEventArgs.SetBuffer(MemoryMarshal.AsMemory(memory));
#endif

            if (!_socket.SendAsync(_awaitableEventArgs))
            {
                _awaitableEventArgs.Complete();
            }

            return _awaitableEventArgs;
        }
    }
}