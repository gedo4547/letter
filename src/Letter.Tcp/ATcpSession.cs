using System.Buffers;
using System.Buffers.Binary;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Letter;

namespace Letter.Tcp
{
    abstract class ATcpSession : ITcpSession
    {
        public ATcpSession(Socket socket, ATcpOptions options, PipeScheduler scheduler, MemoryPool<byte> pool, FilterPipeline<ITcpSession> filterPipeline)
        {
            this.Id = IdGeneratorHelper.GetNextId();
            this.Order = options.Order;
            this.MemoryPool = pool;
            this.Scheduler = scheduler;
            this.MemoryPool = pool;
            this.Scheduler = scheduler;
            
            this.filterPipeline = filterPipeline;
            this.socket = new TcpSocket(socket, scheduler);
            this.LoaclAddress = this.socket.BindAddress;
            this.RemoteAddress = this.socket.RemoteAddress;
            
            long maxReadBufferSize = options.MaxPipelineReadBufferSize == null ? 0 : options.MaxPipelineReadBufferSize.Value;
            long maxWriteBufferSize = options.MaxPipelineWriteBufferSize == null ? 0 : options.MaxPipelineWriteBufferSize.Value;
            var inputOptions = new PipeOptions(this.MemoryPool, PipeScheduler.ThreadPool, scheduler, maxReadBufferSize, maxReadBufferSize / 2, useSynchronizationContext: false);
            var outputOptions = new PipeOptions(this.MemoryPool, scheduler, PipeScheduler.ThreadPool, maxWriteBufferSize, maxWriteBufferSize / 2, useSynchronizationContext: false);

            
            this.SettingSocket(this.socket, options);
        }
        
        public string Id { get; }
        public BinaryOrder Order { get; }
        public EndPoint LoaclAddress { get; }
        public EndPoint RemoteAddress { get; }
        public MemoryPool<byte> MemoryPool { get; }
        public PipeScheduler Scheduler { get; }
        
        private TcpSocket socket;

        protected FilterPipeline<ITcpSession> filterPipeline;
        
        
        public abstract Task StartAsync();

        public abstract Task WriteAsync(object obj);
        
        private void SettingSocket(TcpSocket socket, ATcpOptions options)
        {
            this.socket.SettingKeepAlive(options.KeepAlive);
            this.socket.SettingLingerState(options.LingerOption);
            this.socket.SettingNoDelay(options.NoDelay);

            if (options.RcvTimeout != null)
                this.socket.SettingRcvTimeout(options.RcvTimeout.Value);
            if (options.SndTimeout != null)
                this.socket.SettingSndTimeout(options.SndTimeout.Value);
            if (options.RcvBufferSize != null)
                this.socket.SettingRcvBufferSize(options.RcvBufferSize.Value);
            if (options.SndBufferSize != null)
                this.socket.SettingSndBufferSize(options.SndBufferSize.Value);
        }

        private void SettingPipeline()
        {
            
        }







        public virtual ValueTask DisposeAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}