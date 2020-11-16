using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;

namespace System.Net.Sockets
{
    public sealed class TcpSocket : ASocket
    {
        private static Memory<byte> EmptyMemory = Memory<byte>.Empty;
        
        public TcpSocket(Socket socket, PipeScheduler scheduler) : base(socket, scheduler)
        {
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SettingLingerState(LingerOption option)
        {
            this.socket.LingerState = option;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SettingNoDelay(bool noDelay)
        {
            this.socket.NoDelay = noDelay;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SettingKeepAlive(bool keepAlive)
        {
            this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, keepAlive);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SocketAwaitableArgs Wait()
        {
            return base.InternalReceiveAsync(EmptyMemory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SocketAwaitableArgs ReceiveAsync(Memory<byte> memory)
        {
            return base.InternalReceiveAsync(memory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SocketAwaitableArgs SendAsync(ReadOnlySequence<byte> buffers)
        {
            return base.InternalSendAsync(buffers);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool SocketRcvOperationAsync(SocketAwaitableArgs args)
        {
            return this.socket.ReceiveAsync(args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool SocketSndOperationAsync(SocketAwaitableArgs args)
        {
            return this.socket.SendAsync(args);
        }
    }
}