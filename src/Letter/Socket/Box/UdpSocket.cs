using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace Letter
{
    public sealed class UdpSocket : ASocket
    {
        public UdpSocket(Socket socket, PipeScheduler scheduler) : base(socket, scheduler)
        {
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SocketAwaitableArgs ReceiveAsync(EndPoint loaclAddress, ref Memory<byte> memory)
        {
            this.rcvArgs.RemoteEndPoint = loaclAddress;
            return base.InternalReceiveAsync(ref memory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SocketAwaitableArgs SendAsync(EndPoint remoteAddress, ref ReadOnlySequence<byte> buffers)
        {
            this.sndArgs.RemoteEndPoint = remoteAddress;
            return base.InternalSendAsync(ref buffers);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool SocketAsyncRcvOperation(SocketAwaitableArgs args)
        {
            return this.socket.ReceiveFromAsync(args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool SocketAsyncSndOperation(SocketAwaitableArgs args)
        {
            return this.socket.SendToAsync(args);
        }
    }
}