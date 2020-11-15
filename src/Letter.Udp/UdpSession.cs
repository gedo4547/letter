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

            try
            {
                this.filterPipeline.OnTransportActive(this);
            }
            catch (Exception e)
            {
                this.filterPipeline.OnTransportException(this, e);
                this.DisposeAsync().NoAwait();
            }
        }
        
        private async Task SocketReceiveAsync()
        {
            var address = this.LocalAddress;
            ASegment segment = null;
            try
            {
                while (true)
                {
                    segment = this.RcvPipeWriter.GetSegment();
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
                    segment = null;
                }
            }
            catch (ObjectDisposedException)
            {
                
            }
            catch (Exception ex)
            {
                this.filterPipeline.OnTransportException(this, ex);
                this.DisposeAsync().NoAwait();
            }
            finally
            {
                if (segment != null) segment.Reset();
            }
        }

        private void OnRcvPipelineRead()
        {
            ReadDgramResult readDgramResult = RcvPipeReader.Read();
            if (readDgramResult.IsEmpty)
            {
                return;
            }

            bool isError = false;
            ASegment head = readDgramResult.Head;
            ASegment tail = readDgramResult.Tail;
            ASegment segment = null;

            while (head != null)
            {
                //这里发生异常后，将关闭socket，停止向filter传递数据，将现有的segment回收
                if (!isError)
                {
                    this.RcvAddress = (EndPoint) head.Token;
                    var memory = head.GetReadableMemory();
                    var w_reader = new WrappedReader(new ReadOnlySequence<byte>(memory), this.Order, this.readerFlushCallback);
                    try
                    {
                        this.filterPipeline.OnTransportRead(this, ref w_reader);
                    }
                    catch (Exception ex)
                    {
                        isError = true;
                        this.filterPipeline.OnTransportException(this, ex);
                        this.DisposeAsync().NoAwait();
                    }
                }

                segment = head;
                head = head.ChildSegment;
                segment.Reset();
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
                try
                {
                    this.filterPipeline.OnTransportWrite(this, ref writer, o);
                }
                catch (Exception e)
                {
                    this.filterPipeline.OnTransportException(this, e);
                    this.DisposeAsync().NoAwait();
                }
                finally
                {
                    writer.Flush();
                }
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

            bool isError = false;
            ASegment head = readDgramResult.Head;
            ASegment tail = readDgramResult.Tail;
            ASegment segment = null;

            while (head != null)
            {
                if (!isError)
                {
                    ReadOnlyMemory<byte> memory = head.GetReadableMemory();
                    if (memory.Length > 0)
                    {
                        EndPoint address = (EndPoint) head.Token;
                        ReadOnlySequence<byte> sequence = new ReadOnlySequence<byte>(memory);
                        try
                        {
                            SocketResult socketResult = await this.socket.SendAsync(address, ref sequence);
                            if (this.SocketErrorNotify(socketResult.error))
                            {
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            isError = true;
                            this.filterPipeline.OnTransportException(this, e);
                            this.DisposeAsync().NoAwait();
                        }
                    }
                }
                   
                segment = head;
                head = head.ChildSegment;
                segment.Reset();
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
            Console.WriteLine("session 关闭");
            this.isDisposed = true;

            this.filterPipeline.OnTransportInactive(this);
            await this.socket.DisposeAsync();
            
            await this.readTask;
            
            this.rcvPipeline.Dispose();
            this.sndPipeline.Dispose();
        }
    }
}