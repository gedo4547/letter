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
        public UdpSession(
            Socket socket, 
            UdpOptions options, 
            MemoryPool<byte> memoryPool, 
            PipeScheduler scheduler, 
            FilterPipeline<IUdpSession> filterPipeline)
        {
            this.Id = IdGeneratorHelper.GetNextId();
            this.socket = new UdpSocket(socket, scheduler);
            this.SettingSocket(this.socket, options);
            
            this.Order = options.Order;
            this.Scheduler = scheduler;
            this.MemoryPool = memoryPool;
            this.filterPipeline = filterPipeline;
            this.LocalAddress = this.socket.BindAddress;
            
            this.rcvPipeline = new DgramPipeline(this.MemoryPool, scheduler, OnRcvPipelineRead);
            this.sndPipeline = new DgramPipeline(this.MemoryPool, scheduler, OnSndPipelineRead);

            this.readerFlushCallback = (startPos, endPos) => { };
            this.writerFlushCallback = (writer) =>
            {
                this.SndPipeWriter.Write((DgramNode) writer);
            };
        }

        private UdpSocket socket;
        private FilterPipeline<IUdpSession> filterPipeline;
        private DgramPipeline sndPipeline;
        private DgramPipeline rcvPipeline;
        private ReaderFlushDelegate readerFlushCallback;
        private WriterFlushDelegate writerFlushCallback;
        
        private object sync = new object();
        
        public string Id { get; }
        public BinaryOrder Order { get; }
        public EndPoint LocalAddress { get; }
        public EndPoint RcvAddress { get; private set; }
        public EndPoint SndAddress { get; private set; }
        public EndPoint RemoteAddress
        {
            get { throw new Exception("please use IUdpSession.RcvAddress or IUdpSession.SndAddress"); }
        }
        public MemoryPool<byte> MemoryPool { get; }
        public PipeScheduler Scheduler { get; }

        public IDgramPipelineReader RcvPipeReader
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.rcvPipeline; }
        }
        
        public IDgramPipelineWriter RcvPipeWriter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.rcvPipeline; }
        }
        
        public IDgramPipelineReader SndPipeReader
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.sndPipeline; }
        }

        public IDgramPipelineWriter SndPipeWriter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this.sndPipeline; }
        }
        
        public void Start()
        {
            this.sndPipeline.ReceiveAsync();
            this.rcvPipeline.ReceiveAsync();
        }
        
        private async Task SocketReceiveAsync()
        {
            while (true)
            {
                var node = this.RcvPipeWriter.GetDgramNode();
                var memory = node.GetMomory();
                try
                {
                    int transportBytes = await this.socket.ReceiveAsync(this.LocalAddress, ref memory);
                    node.SettingPoint(this.socket.RemoteAddress);
                    node.SettingWriteLength(transportBytes);
                    this.RcvPipeWriter.Write(node);
                }
                catch(Exception ex)
                {
                    await node.ReleaseAsync();
                    if (SocketErrorHelper.IsSocketDisabledError(ex) || ex is ObjectDisposedException)
                    {
                        await this.DisposeAsync();
                    }
                    else
                    {
                        this.filterPipeline.OnTransportException(this, ex);
                    }
                    return;
                }
            }
        }

        private void OnRcvPipelineRead(IDgramPipelineReader reader)
        {
            while (true)
            {
                DgramNode node = reader.Read();
                if (node == null) break;

                this.RcvAddress = node.Point;
                Memory<byte> memory = node.GetReadableBuffer();
                var w_reader = new WrappedReader(new ReadOnlySequence<byte>(memory), this.Order, this.readerFlushCallback);
                this.filterPipeline.OnTransportRead(this, ref w_reader);
                node.ReleaseAsync().NoAwait();
            }
            
            this.RcvPipeReader.ReceiveAsync();
        }
        
        public Task WriteAsync(EndPoint remoteAddress, object obj)
        {
            lock (sync)
            {
                this.SndAddress = remoteAddress;
                var node = this.SndPipeWriter.GetDgramNode();
                node.SettingPoint(remoteAddress);
                var writer = new WrappedWriter(node, this.Order, this.writerFlushCallback);
                this.filterPipeline.OnTransportWrite(this, ref writer, null);

                return Task.CompletedTask;
            }
        }

        private async void OnSndPipelineRead(IDgramPipelineReader reader)
        {
            while (true)
            {
                var node = reader.Read();
                if (node == null) break;

                var memory = node.GetReadableBuffer();
                var buffer = new ReadOnlySequence<byte>(memory);
                var address = node.Point;
                
                try
                {
                    await this.socket.SendAsync(address, ref buffer);
                }
                catch (Exception ex)
                {
                    if (!SocketErrorHelper.IsSocketDisabledError(ex) || ex is ObjectDisposedException)
                    {
                        this.filterPipeline.OnTransportException(this, ex);
                    }
                }
                finally
                {
                    await node.ReleaseAsync();
                }
            }
            
            this.SndPipeReader.ReceiveAsync();
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