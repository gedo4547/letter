using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;

namespace Letter
{
    abstract class ASocket : IDisposable
    {
        public ASocket(Socket socket, PipeScheduler scheduler)
        {
            this.socket = socket;
            this.rcvArgs = new SocketAwaitableArgs(scheduler);
            this.sndArgs = new SocketAwaitableArgs(scheduler);
            this.bufferList = new List<ArraySegment<byte>>();
        }
        
        protected Socket socket;
        protected SocketAwaitableArgs rcvArgs;
        protected SocketAwaitableArgs sndArgs;
        
        private List<ArraySegment<byte>> bufferList;

        public EndPoint LocalAddress => this.socket.LocalEndPoint;
        public EndPoint RemoteAddress => this.socket.RemoteEndPoint;

        protected List<ArraySegment<byte>> GetBufferList(ref ReadOnlySequence<byte> buffer)
        {
            this.bufferList.Clear();
            foreach (var b in buffer)
            {
                this.bufferList.Add(b.GetBinaryArray());
            }
            return this.bufferList;
        }
        
        public void Dispose()
        {
            this.socket?.Dispose();
            this.rcvArgs?.Dispose();
            this.sndArgs?.Dispose();
            this.bufferList?.Clear();
        }
    }
}