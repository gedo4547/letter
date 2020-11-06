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
            while (true)
            {
                var segment = this.RcvPipeWriter.GetSegment();
                var memory = segment.GetWritableMemory(this.memoryBlockSize);
                try
                {
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
                Console.WriteLine(">>>>>>>>>buffer>>>>>>>>>>>>>>>>>>");
                this.RcvAddress = (EndPoint) segment.Token;
                var memory = segment.GetReadableMemory();
                var w_reader = new WrappedReader(new ReadOnlySequence<byte>(memory), this.Order, this.readerFlushCallback);
                this.filterPipeline.OnTransportRead(this, ref w_reader);
                this.RcvPipeReader.ReaderAdvance();
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
            // Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            var reader = this.SndPipeReader;
            ReadDgramResult readDgramResult = reader.Read();
            if (readDgramResult.IsEmpty) return;

            var buffer = readDgramResult.GetBuffer();
            
            foreach (var segment in buffer)
            {
                // Console.WriteLine("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                var memory = segment.GetReadableMemory();
                var sequence = new ReadOnlySequence<byte>(memory);
                // Console.WriteLine("ssssssssssssssssssssssssssssss");
                var address = (EndPoint) segment.Token;
                var socketResult = await this.socket.SendAsync(address, ref sequence);
                // Console.WriteLine("dddddddddddddddddddddddddddddd");
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

        public async ValueTask DisposeAsync()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.filterPipeline.OnTransportInactive(this);
            await this.socket.DisposeAsync();
        }
    }
}