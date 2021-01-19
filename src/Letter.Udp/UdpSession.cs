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
            this.writerFlushCallback = (writer) => { };
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
                this.DeliverException(e);
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
                    var socketResult = await this.socket.ReceiveAsync(address, memory);
                    Logger.Info("udp socket receive:"+socketResult.bytesTransferred);
                    if (socketResult.error != SocketError.Success)
                    {
                        if (!SocketErrorHelper.IsSocketDisabledError(socketResult.error))
                            this.DeliverException(new SocketException((int) socketResult.error));
                        else
                            this.CloseAsync().NoAwait();
                    
                        break;
                    }
                    
                    segment.Token = this.socket.RemoteAddress;
                    segment.WriterAdvance(socketResult.bytesTransferred);
                    
                    this.RcvPipeWriter.WriterAdvance();
                    this.RcvPipeWriter.FlushAsync();
                    segment = null;
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception ex)
            {
                this.DeliverException(ex);
            }
            finally
            {
                this.RcvPipeWriter.Complete();
            }
        }

        private void OnRcvPipelineRead()
        {
            ReadDgramResult readDgramResult = RcvPipeReader.Read();
            if (readDgramResult.IsEmpty)
            {
                return;
            }

            int count = 0;
            bool isReadComplete = false;
            ASegment head = readDgramResult.Head;
            ASegment tail = readDgramResult.Tail;
            ASegment segment = head;

            try
            {
                while (!isReadComplete)
                {
                    this.RcvAddress = (EndPoint) segment.Token;
                    var memory = segment.GetReadableMemory();
                    
                    var reader = new WrappedReader(new ReadOnlySequence<byte>(memory), this.Order, this.readerFlushCallback);
                    this.filterPipeline.OnTransportRead(this, ref reader);
                    reader.Flush();
                    
                    count++;
                    if (segment == tail)
                        isReadComplete = true;
                    else
                        segment = segment.ChildSegment;
                }

                this.RcvPipeReader.ReaderAdvance(count);
                this.RcvPipeReader.ReceiveAsync();
            }
            catch (Exception e)
            {
                this.DeliverException(e);
                
                this.RcvPipeReader.Complete();
            }
        }
        
        public void Write(EndPoint remoteAddress, object o)
        {
            lock (this.sync)
            {
                this.SndAddress = remoteAddress;
                var segment = this.SndPipeWriter.GetSegment();
                segment.Token = remoteAddress;
                try
                {
                    var writer = new WrappedWriter(segment, this.Order, this.writerFlushCallback);
                    this.filterPipeline.OnTransportWrite(this, ref writer, o);
                    writer.Flush();
                }
                catch (Exception e)
                {
                    this.DeliverException(e);

                    this.SndPipeWriter.Complete();
                }
            }
        }

        public ValueTask FlushAsync()
        {
            this.SndPipeWriter.WriterAdvance();
            this.SndPipeWriter.FlushAsync();
            return default;
        }

        private async void OnSndPipelineRead()
        {
            ReadDgramResult readDgramResult = this.SndPipeReader.Read();
            if (readDgramResult.IsEmpty)
            {
                return;
            }

            int count = 0;
            bool isReadComplete = false;
            ASegment head = readDgramResult.Head;
            ASegment tail = readDgramResult.Tail;
            ASegment segment = head;
            try
            {
                while (!isReadComplete)
                {
                    ReadOnlyMemory<byte> memory = segment.GetReadableMemory();
                    if (memory.Length > 0)
                    {
                        EndPoint address = (EndPoint) segment.Token;
                        ReadOnlySequence<byte> sequence = new ReadOnlySequence<byte>(memory);
                        SocketResult socketResult = await this.socket.SendAsync(address, sequence);
                        Logger.Info("udp socket send："+socketResult.bytesTransferred);
                        if (socketResult.error != SocketError.Success)
                        {
                            if (!SocketErrorHelper.IsSocketDisabledError(socketResult.error))
                                this.DeliverException(new SocketException((int) socketResult.error));
                            
                            this.SndPipeReader.Complete();
                            break;
                        }
                    }
                    count++;
                    if (segment == tail)
                        isReadComplete = true;
                    else
                        segment = segment.ChildSegment;
                }

                this.SndPipeReader.ReaderAdvance(count);
                this.SndPipeReader.ReceiveAsync();
            }
            catch (Exception e)
            {
                this.DeliverException(e);

                this.SndPipeReader.Complete();
            }
        }
        
        private void DeliverException(Exception ex)
        {
            this.filterPipeline.OnTransportException(this, ex);
            this.CloseAsync().NoAwait();
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

        public async Task CloseAsync()
        {
            if (this.isDisposed)
            {
                return;
            }
            this.isDisposed = true;

            this.filterPipeline.OnTransportInactive(this);
            await this.socket.DisposeAsync();
            
            await this.readTask;
            
            this.sndPipeline.Complete();

#if DEBUG
            Logger.Info("session close");
#endif
            
        }
    }
}