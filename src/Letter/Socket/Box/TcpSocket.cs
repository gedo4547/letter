using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

#if NET5_0
using System.Runtime.InteropServices;
#endif

namespace Letter
{
    public sealed class TcpSocket : ASocket
    {
        public TcpSocket(Socket socket, PipeScheduler scheduler) : base(socket, scheduler)
        {
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SettingLingerState(LingerOption option) => this.socket.LingerState = option;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SettingNoDelay(bool noDelay) => this.socket.NoDelay = noDelay;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SettingKeepAlive(bool keepAlive)
            => this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, keepAlive);

        public SocketAwaitableArgs ReceiveAsync(ref Memory<byte> memory)
        {
#if NETSTANDARD2_0
            ArraySegment<byte> segment = memory.GetBinaryArray();
            this.rcvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);
#elif NET5_0
            this.rcvArgs.SetBuffer(memory);
#endif
            if (!this.socket.ReceiveAsync(this.rcvArgs))
            {
                this.rcvArgs.Complete();
            }

            return this.rcvArgs;
        }

        public SocketAwaitableArgs SendAsync(ref ReadOnlySequence<byte> buffers)
        {
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
            if (!this.socket.SendAsync(this.sndArgs))
            {
                this.sndArgs.Complete();
            }

            return this.sndArgs;
        }

        private SocketAwaitableArgs SendMultipleMessageAsync(ref ReadOnlySequence<byte> buffers)
        {
#if NETSTANDARD2_0
            if (!Array.Empty<byte>().Equals(this.sndArgs.Buffer))
            {
                this.sndArgs.SetBuffer(null, 0, 0);
            }
#elif NET5_0
            if (!this.sndArgs.MemoryBuffer.Equals(Memory<byte>.Empty))
            {
                this.sndArgs.SetBuffer(null, 0, 0);
            }
#endif
            
            this.sndArgs.BufferList = this.GetBufferList(ref buffers);
            if (!this.socket.SendAsync(this.sndArgs))
            {
                this.sndArgs.Complete();
            }

            return sndArgs;
        }
    }
}