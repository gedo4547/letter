using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Letter
{
    public abstract class ASocket : IAsyncDisposable
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

        public EndPoint RemoteAddress => this.rcvArgs.RemoteEndPoint;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SettingRcvBufferSize(int rcvBufferSize) => this.socket.ReceiveBufferSize = rcvBufferSize;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SettingSndBufferSize(int sndBufferSize) => this.socket.SendBufferSize = sndBufferSize;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SettingRcvTimeout(int rcvTimeout) => this.socket.ReceiveTimeout = rcvTimeout;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SettingSndTimeout(int sndTimeout) => this.socket.SendTimeout = sndTimeout;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SettingReuseAddress(bool reuseAddress)
            => this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, reuseAddress);
        
        protected List<ArraySegment<byte>> GetBufferList(ref ReadOnlySequence<byte> buffer)
        {
            this.bufferList.Clear();
            foreach (var b in buffer)
            {
                this.bufferList.Add(b.GetBinaryArray());
            }
            return this.bufferList;
        }

        public ValueTask DisposeAsync()
        {
            if (this.socket != null)
            {
                try
                {
                    this.socket.Shutdown(SocketShutdown.Both);
                }
                catch
                {
                }
                
                this.socket.Dispose();
            }
            
            this.rcvArgs?.Dispose();
            this.sndArgs?.Dispose();
            this.bufferList?.Clear();

            return default;
        }
    }
}