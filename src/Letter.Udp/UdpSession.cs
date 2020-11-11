using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Letter.IO;

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
                this.SndPipeWriter.WriterAdvance((ASegment) writer);
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
        protected volatile bool isDisposed = false;
        
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
            
            this.filterPipeline.OnTransportActive(this);
        }
        
        private async Task SocketReceiveAsync()
        {
            var address = this.LocalAddress;
            try
            {
                while (true)
                {
                    var segment = this.RcvPipeWriter.GetSegment();
                    var memory = segment.GetWritableMemory(this.memoryBlockSize);
                    var socketResult = await this.socket.ReceiveAsync(address, ref memory);
                    if (this.SocketErrorNotify(socketResult.error))
                    {
                        break;
                    }
                    
                    segment.Token = this.socket.RemoteAddress;
                    segment.WriterAdvance(socketResult.bytesTransferred);
                    this.RcvPipeWriter.WriterAdvance(segment);
                    this.RcvPipeWriter.FlushAsync();
                }
            }
            catch(ObjectDisposedException)
            {
                    
            }
        }

        private void OnRcvPipelineRead()
        {
            ReadDgramResult readDgramResult = RcvPipeReader.Read();
            if (readDgramResult.IsEmpty)
            {
                return;
            }
            
            ASegment head = readDgramResult.Head;
            ASegment tail = readDgramResult.Tail;
            ASegment segment = null;

            while (head != null)
            {
                this.RcvAddress = (EndPoint) head.Token;
                var memory = head.GetReadableMemory();
                var w_reader = new WrappedReader(new ReadOnlySequence<byte>(memory), this.Order, this.readerFlushCallback);
                this.filterPipeline.OnTransportRead(this, ref w_reader);
                
                segment = head;
                head = head.ChildSegment;
                segment.Release();
            }
            
            this.RcvPipeReader.ReceiveAsync();
        }
        
        public void Write(EndPoint remoteAddress, object o)
        {
            lock (this.sync)
            {
                this.SndAddress = remoteAddress;
                var segment = this.SndPipeWriter.GetSegment();
                segment.Token = remoteAddress;
                var writer = new WrappedWriter(segment, this.Order, this.writerFlushCallback);
                this.filterPipeline.OnTransportWrite(this, ref writer, o);
                writer.Flush();
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
            
            ASegment head = readDgramResult.Head;
            ASegment tail = readDgramResult.Tail;
            ASegment segment = null;

            try
            {
                while (head != null)
                {
                    ReadOnlyMemory<byte> memory = head.GetReadableMemory();
                    if (memory.Length > 0)
                    {
                        EndPoint address = (EndPoint) head.Token;
                        ReadOnlySequence<byte> sequence = new ReadOnlySequence<byte>(memory);
                        SocketResult socketResult = await this.socket.SendAsync(address, ref sequence);
                        if (this.SocketErrorNotify(socketResult.error))
                        {
                            break;
                        }
                    }
                    segment = head;
                    head = head.ChildSegment;
                    segment.Release();
                }

                this.SndPipeReader.ReceiveAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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

        public async ValueTask DisposeAsync()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.filterPipeline.OnTransportInactive(this);
            await this.socket.DisposeAsync();
            
            await this.readTask;
            
            this.rcvPipeline.Dispose();
            this.sndPipeline.Dispose();
        }
    }
}