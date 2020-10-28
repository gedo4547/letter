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
            this.readerFlushCallback = OnFilterReadFlush;
            this.writerFlushCallback = (writer) => { };
        }
        
        private StreamPipelineReader Input
        {
            get { return base.Transport.WrappedInput; }
        }

        private StreamPipelineWriter Output
        {
            get { return base.Transport.WrappedOutput; }
        }

        private object sync = new object();
        private ReaderFlushDelegate readerFlushCallback;
        private WriterFlushDelegate writerFlushCallback;

        private Task readTask;
        
        public override Task StartAsync()
        {
            this.readTask = ReadReceiveBuffer();
            
            base.Run();
            
            return Task.CompletedTask;
        }

        private async Task ReadReceiveBuffer()
        {
            StreamPipelineReader input = this.Input;
            while (true)
            {
                ReadResult result = await input.ReadAsync();
                if (result.IsCanceled || result.IsCompleted)
                {
                    break;
                }
                var buffer = result.Buffer;
                
                WrappedReader reader = new WrappedReader(buffer, Order, this.readerFlushCallback);
                this.filterPipeline.OnTransportRead(this, ref reader);
            }
        }
        
        private void OnFilterReadFlush(SequencePosition startpos, SequencePosition endpos)
        {
            this.Input.AdvanceTo(startpos, endpos);
        }

        public override void Write(object o)
        {
            lock (sync)
            {
                WrappedWriter writer = new WrappedWriter(this.Output, this.Order, this.writerFlushCallback);
                this.filterPipeline.OnTransportWrite(this, ref writer, o);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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