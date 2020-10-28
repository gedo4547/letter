using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#if NET5_0
using System.Runtime.InteropServices;
#endif

namespace System.Net.Sockets
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
        
        public EndPoint BindAddress
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.socket.LocalEndPoint; }
        }
        
        public EndPoint RemoteAddress
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.rcvArgs.RemoteEndPoint; }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SettingRcvBufferSize(int rcvBufferSize)
        {
            this.socket.ReceiveBufferSize = rcvBufferSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SettingSndBufferSize(int sndBufferSize)
        {
            this.socket.SendBufferSize = sndBufferSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SettingRcvTimeout(int rcvTimeout)
        {
            this.socket.ReceiveTimeout = rcvTimeout;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SettingSndTimeout(int sndTimeout)
        {
            this.socket.SendTimeout = sndTimeout;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SettingReuseAddress(bool reuseAddress)
        {
            this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, reuseAddress);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected SocketAwaitableArgs InternalReceiveAsync(ref Memory<byte> memory)
        {
#if NETSTANDARD2_0
            ArraySegment<byte> segment = memory.GetBinaryArray();
            this.rcvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);
#elif NET5_0
            this.rcvArgs.SetBuffer(memory);
#endif
            if (!this.SocketAsyncRcvOperation(this.rcvArgs))
            {
                this.rcvArgs.Complete();
            }

            return this.rcvArgs;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected SocketAwaitableArgs InternalSendAsync(ref ReadOnlySequence<byte> buffers)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            if (!this.SocketAsyncSndOperation(this.sndArgs))
            {
                this.sndArgs.Complete();
            }

            return this.sndArgs;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                sndArgs.SetBuffer(null, 0, 0);
            }
#endif
            
            this.sndArgs.BufferList = this.GetBufferList(ref buffers);
            if (!this.SocketAsyncSndOperation(this.sndArgs))
            {
                this.sndArgs.Complete();
            }

            return sndArgs;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract bool SocketAsyncRcvOperation(SocketAwaitableArgs args);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract bool SocketAsyncSndOperation(SocketAwaitableArgs args);

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