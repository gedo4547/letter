using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    class TcpSession : ATcpSession
    {
        public TcpSession(Socket socket, ATcpOptions options, PipeScheduler scheduler, MemoryPool<byte> pool, FilterPipeline<ITcpSession> filterPipeline) 
            : base(socket, options, scheduler, pool, filterPipeline)
        {
            Console.WriteLine("TcpSession");
            this.readerFlushCallback = OnFilterReadFlush;
            this.writerFlushCallback = (writer) => { };
        }
        
        protected virtual StreamPipelineReader Input
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return base.Transport.WrappedInput; }
        }

        protected virtual StreamPipelineWriter Output
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return base.Transport.WrappedOutput; }
        }

        private object sync = new object();
        private ReaderFlushDelegate readerFlushCallback;
        private WriterFlushDelegate writerFlushCallback;

        protected Task readTask;
        
        public override Task StartAsync()
        {
            base.Run();
            this.readTask = this.ReadBufferAsync();
            
            return Task.CompletedTask;
        }

        protected async Task ReadBufferAsync()
        {
            StreamPipelineReader input = this.Input;
            while (!base.isDisposed)
            {
                Console.WriteLine("Transport        000000000000");
                ReadResult result = await input.ReadAsync();
                Console.WriteLine("Transport        111111111");
                if (result.IsCanceled || result.IsCompleted)
                {
                    break;
                }

                this.TransportReadNotify(result.Buffer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TransportReadNotify(ReadOnlySequence<byte> buffer)
        {
            var reader = new WrappedReader(buffer, this.Order, this.readerFlushCallback);
            Console.WriteLine("Transport        拋出");
            this.filterPipeline.OnTransportRead(this, ref reader);
        }

        private void OnFilterReadFlush(SequencePosition startpos, SequencePosition endpos)
        {
            this.Input.AdvanceTo(startpos, endpos);
        }

        public override void Write(object o)
        {
            lock (sync)
            {
                if (this.isDisposed) return;

                WrappedWriter writer = new WrappedWriter(this.Output, this.Order, this.writerFlushCallback);
                this.filterPipeline.OnTransportWrite(this, ref writer, o);
            }
        }
        
        public async override Task FlushAsync()
        {
            FlushResult result = await this.Output.FlushAsync();
            if (result.IsCompleted || result.IsCanceled)
            {
                this.Output.Complete();
            }
        }
        
        public override ValueTask DisposeAsync()
        {
            return base.DisposeAsync();
        }
    }
}