using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;

#if NET5_0
using System.Runtime.InteropServices;
#endif

namespace Letter
{
    public sealed class UdpSocket : ASocket
    {
        public UdpSocket(Socket socket, PipeScheduler scheduler) : base(socket, scheduler)
        {
        }
        
        public SocketAwaitableArgs ReceiveAsync(EndPoint loaclAddress, ref Memory<byte> memory)
        {
            this.rcvArgs.RemoteEndPoint = loaclAddress;
#if NETSTANDARD2_0
            ArraySegment<byte> segment = memory.GetBinaryArray();
            this.rcvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);
#elif NET5_0
            this.rcvArgs.SetBuffer(memory);
#endif
            if (!this.socket.ReceiveFromAsync(this.rcvArgs))
            {
                this.rcvArgs.Complete();
            }

            return rcvArgs;
        }

        public SocketAwaitableArgs SendAsync(EndPoint remoteAddress, ref ReadOnlySequence<byte> buffers)
        {
            this.sndArgs.RemoteEndPoint = remoteAddress;
            if (buffers.IsSingleSegment)
            {
                var memory = buffers.First;
                return this.SendSingleMessageAsync(ref memory);
            }
            else
            {
                return this.SendMultipleMessageAsync(ref buffers);
            }
        }

        private SocketAwaitableArgs SendSingleMessageAsync(ref ReadOnlyMemory<byte> buffer)
        {
            if (this.sndArgs.BufferList != null)
            {
                this.sndArgs.BufferList = null;
            }
#if NETSTANDARD2_0
            var segment = buffer.GetBinaryArray();
            this.sndArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);
#elif NET5_0
            this.sndArgs.SetBuffer(MemoryMarshal.AsMemory(buffer));
#endif
            if (!this.socket.SendToAsync(this.sndArgs))
            {
                this.sndArgs.Complete();
            }

            return sndArgs;
        }

        private SocketAwaitableArgs SendMultipleMessageAsync(ref ReadOnlySequence<byte> buffers)
        {
#if NETSTANDARD2_0
            if (!Array.Empty<byte>().Equals(this.sndArgs.Buffer))
            {
                this.sndArgs.SetBuffer(null, 0, 0);
            }
#elif NET5_0
            if (!sndArgs.MemoryBuffer.Equals(Memory<byte>.Empty))
            {
                this.sndArgs.SetBuffer(null, 0, 0);
            }
#endif
            this.sndArgs.BufferList = this.GetBufferList(ref buffers);
            if (!this.socket.SendToAsync(this.sndArgs))
            {
                this.sndArgs.Complete();
            }

            return sndArgs;
        }
    }
}