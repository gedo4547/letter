using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Letter.Udp
{
    public class UdpSession : IUdpSession
    {
        public UdpSession(Socket socket, UdpOptions options, MemoryPool<byte> memoryPool, PipeScheduler scheduler, FilterPipeline<IUdpSession> filterPipeline)
        {
            this.Id = IdGeneratorHelper.GetNextId();
            this.socket = new UdpSocket(socket, scheduler);
            this.SettingSocket(this.socket, options);
            
            this.Order = options.Order;
            this.Scheduler = scheduler;
            this.MemoryPool = memoryPool;
            this.filterPipeline = filterPipeline;
            this.LocalAddress = this.socket.BindAddress;
            this.memoryBlockSize = this.MemoryPool.MaxBufferSize;
            
            this.rcvPipeline = new DgramPipeline(this.MemoryPool, scheduler, OnRcvPipelineRead);
            this.sndPipeline = new DgramPipeline(this.MemoryPool, scheduler, OnSndPipelineRead);

            this.readerFlushCallback = (startPos, endPos) => { };
            this.writerFlushCallback = (writer) =>
            {
                // this.SndPipeWriter.Write((DgramNode) writer);
            };
        }
        
        public string Id { get; }
        public BinaryOrder Order { get; }
        public EndPoint LocalAddress { get; }
        public EndPoint RcvAddress { get; private set; }
        public EndPoint SndAddress { get; private set; }
        public MemoryPool<byte> MemoryPool { get; }
        public PipeScheduler Scheduler { get; }

        private UdpSocket socket;
        private DgramPipeline sndPipeline;
        private DgramPipeline rcvPipeline;
        private ReaderFlushDelegate readerFlushCallback;
        private WriterFlushDelegate writerFlushCallback;
        private FilterPipeline<IUdpSession> filterPipeline;

        private int memoryBlockSize;
        private object sync = new object();
        private Task readTask;

        public DgramPipelineReader RcvPipeReader
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.rcvPipeline.Reader; }
        }
        
        public DgramPipelineWriter RcvPipeWriter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.rcvPipeline.Writer; }
        }
        
        public DgramPipelineReader SndPipeReader
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.sndPipeline.Reader; }
        }

        public DgramPipelineWriter SndPipeWriter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.sndPipeline.Writer; }
        }
        
        public void Start()
        {
            this.sndPipeline.ReceiveAsync();
            this.rcvPipeline.ReceiveAsync();

            this.readTask = this.SocketReceiveAsync();
        }
        
        private async Task SocketReceiveAsync()
        {
            while (true)
            {
                var node = this.RcvPipeWriter.GetSegment();
                var memory = node.GetWritableMemory(this.memoryBlockSize);
                try
                {
                    var socketResult = await this.socket.ReceiveAsync(this.LocalAddress, ref memory);
                    if (this.SocketErrorNotify(socketResult.error))
                    {
                        break;
                    }
                    
                    node.Token = this.socket.RemoteAddress;
                    node.WriterAdvance(socketResult.bytesTransferred);
                    this.RcvPipeWriter.WriterAdvance(node);
                }
                catch(ObjectDisposedException)
                {
                    
                }
            }
        }

        private void OnRcvPipelineRead()
        {
            ReadDgramResult readDgramResult = RcvPipeReader.Read();
            if (readDgramResult.IsEmpty)
            {
                return;
            }

            var buffer = readDgramResult.GetBuffer();
            foreach (var segment in buffer)
            {
                this.RcvAddress = (EndPoint) segment.Token;
                var memory = segment.GetReadableMemory();
                var w_reader = new WrappedReader(new ReadOnlySequence<byte>(memory), this.Order, this.readerFlushCallback);
                this.filterPipeline.OnTransportRead(this, ref w_reader);
                RcvPipeReader.ReaderAdvance();
            }

            this.RcvPipeReader.ReceiveAsync();
        }
        
        public Task WriteAsync(EndPoint remoteAddress, object obj)
        {
            lock (sync)
            {
                this.SndAddress = remoteAddress;
                var segment = this.SndPipeWriter.GetSegment();
                segment.Token = remoteAddress;
                var writer = new WrappedWriter(segment, this.Order, this.writerFlushCallback);
                this.filterPipeline.OnTransportWrite(this, ref writer, null);

                return Task.CompletedTask;
            }
        }

        public Task FlushAsync()
        {
            this.SndPipeWriter.FlushAsync();
            return Task.CompletedTask;
        }

        private async void OnSndPipelineRead()
        {
            var reader = this.SndPipeReader;
            ReadDgramResult readDgramResult = reader.Read();
            if (readDgramResult.IsEmpty) return;

            var buffer = readDgramResult.GetBuffer();
            
            foreach (var segment in buffer)
            {
                var memory = segment.GetReadableMemory();
                var sequence = new ReadOnlySequence<byte>(memory);
                var socketResult = await this.socket.SendAsync((EndPoint)segment.Token, ref sequence);
                this.SndPipeReader.ReaderAdvance();
                if (this.SocketErrorNotify(socketResult.error))
                {
                    break;
                }
            }

            this.SndPipeReader.ReceiveAsync();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool SocketErrorNotify(SocketError error)
        {
            if (error != SocketError.Success)
            {
                if (!SocketErrorHelper.IsSocketDisabledError(error))
                {
                    this.DisposeAsync().NoAwait();
                    this.filterPipeline.OnTransportException(this, new SocketException((int)error));
                }

                return true;
            }

            return false;
        }
        
        private void SettingSocket(UdpSocket socket, UdpOptions options)
        {
            if (options.RcvTimeout != null)
                socket.SettingRcvTimeout(options.RcvTimeout.Value);
            if (options.SndTimeout != null)
                socket.SettingSndTimeout(options.SndTimeout.Value);
            
            if (options.RcvBufferSize != null)
                socket.SettingRcvBufferSize(options.RcvBufferSize.Value);
            if (options.SndBufferSize != null)
                socket.SettingSndBufferSize(options.SndBufferSize.Value);
        }

        public ValueTask DisposeAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}