using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SocketAwaitableArgs ReceiveAsync(ref Memory<byte> memory)
        {
            return base.InternalReceiveAsync(ref memory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SocketAwaitableArgs SendAsync(ref ReadOnlySequence<byte> buffers)
        {
            return base.InternalSendAsync(ref buffers);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool SocketAsyncRcvOperation(SocketAwaitableArgs args)
        {
            return this.socket.ReceiveAsync(args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool SocketAsyncSndOperation(SocketAwaitableArgs args)
        {
            return this.socket.SendAsync(args);
        }
    }
}