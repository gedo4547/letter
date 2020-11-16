using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;

namespace System.Net.Sockets
{
    public sealed class UdpSocket : ASocket
    {
        public UdpSocket(Socket socket, PipeScheduler scheduler) : base(socket, scheduler)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SocketAwaitableArgs ReceiveAsync(EndPoint loaclAddress, Memory<byte> memory)
        {
            this.rcvArgs.RemoteEndPoint = loaclAddress;
            return base.InternalReceiveAsync(memory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SocketAwaitableArgs SendAsync(EndPoint remoteAddress, ReadOnlySequence<byte> buffers)
        {
            this.sndArgs.RemoteEndPoint = remoteAddress;
            return base.InternalSendAsync(buffers);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool SocketRcvOperationAsync(SocketAwaitableArgs args)
        {
            return this.socket.ReceiveFromAsync(args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool SocketSndOperationAsync(SocketAwaitableArgs args)
        {
            return this.socket.SendToAsync(args);
        }
    }
}