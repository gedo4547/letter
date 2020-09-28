using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    class TcpClient : ATcpNetwork<TcpClientOptions>, ITcpClient
    {
        static readonly bool IsWindows = OSPlatformHelper.IsWindows();
        
        public TcpClient() : base(new TcpClientOptions())
        {
        }
        
        public string Id { get; private set; }
        public EndPoint LocalAddress { get; private set; }
        public EndPoint RemoteAddress { get; private set; }
        public IDuplexPipe Transport { get; private set; }
        public IDuplexPipe Application { get; private set; }
        public MemoryPool<byte> MemoryPool { get; private set; }
        public PipeScheduler Scheduler{get; private set;}
       
        
        
        private Socket connectSocket;
        private TcpSocketReceiver receiver;
        private TcpSocketSender sender;
        private bool waitForData;
        private int minAllocBufferSize;
        private Task _processingTask;
    
        private volatile bool isDisposed = false;

        private Action<ITcpClient> onClosed;
        private Action<Exception> onException;
        
        public PipeWriter Input => Application.Output;
        public PipeReader Output => Application.Input;
        

        public void AddClosedListener(Action<ITcpClient> onClosed)
        {
            if(onClosed == null)
                 throw new ArgumentNullException(nameof(onClosed));
            this.onClosed += onClosed;
        }

        public void AddExceptionListener(Action<Exception> onException)
        {
            if (onException == null)
                throw new ArgumentNullException(nameof(onException));
            this.onException += onException;
        }


        public void Start(Socket socket, PipeScheduler scheduler)
        {
            if (socket is null)
                throw new ArgumentNullException(nameof(socket));

            this.connectSocket = socket;
            this.StartConfigurationConnect(scheduler);

            this.Run();
        }

        public async ValueTask ConnectAsync(EndPoint endpoint, CancellationToken cancellationToken = default)
        {
            var ipEndPoint = endpoint as IPEndPoint;
            if (ipEndPoint is null)
                throw new NotSupportedException("The SocketConnectionFactory only supports IPEndPoints for now.");

            if (connectSocket == null)
            {
                this.connectSocket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                this.connectSocket.NoDelay = this.options.NoDelay;
            }
            this.StartConfigurationConnect(PipeScheduler.ThreadPool);
            await this.connectSocket.ConnectAsync(ipEndPoint);

            this.Run();
        }

        private void StartConfigurationConnect(PipeScheduler scheduler)
        {
            this.Scheduler = scheduler;
            this.LocalAddress = this.connectSocket.LocalEndPoint;
            this.RemoteAddress = this.connectSocket.RemoteEndPoint;
            this.MemoryPool = this.options.MemoryPoolFactory();
            this.minAllocBufferSize = this.MemoryPool.MaxBufferSize / 2;

            this.connectSocket.SettingNoDelay(this.options.NoDelay);
            this.connectSocket.SettingKeepAlive(this.options.KeepAlive);
            this.connectSocket.SettingLingerState(this.options.LingerOption);

            if (this.options.SndBufferSize != null)
                this.connectSocket.SettingSndBufferSize(this.options.SndBufferSize.Value);
            if (this.options.RcvBufferSize != null)
                this.connectSocket.SettingRcvBufferSize(this.options.RcvBufferSize.Value);
            
            if (this.options.SndTimeout != null)
                this.connectSocket.SettingSndTimeout(this.options.SndTimeout.Value);
            if (this.options.RcvTimeout != null)
                this.connectSocket.SettingRcvTimeout(this.options.RcvTimeout.Value);
            
            this.waitForData = this.options.WaitForDataBeforeAllocatingBuffer;
            var awaiterScheduler = IsWindows ? scheduler : PipeScheduler.Inline;
            this.receiver = new TcpSocketReceiver(this.connectSocket, awaiterScheduler);
            this.sender = new TcpSocketSender(this.connectSocket, awaiterScheduler);
            long maxReadBufferSize = this.options.MaxPipelineReadBufferSize == null ? 0 : this.options.MaxPipelineReadBufferSize.Value;
            long maxWriteBufferSize = this.options.MaxPipelineWriteBufferSize == null ? 0 : this.options.MaxPipelineWriteBufferSize.Value;
            var inputOptions = new PipeOptions(this.MemoryPool, PipeScheduler.ThreadPool, scheduler, maxReadBufferSize, maxReadBufferSize / 2, useSynchronizationContext: false);
            var outputOptions = new PipeOptions(this.MemoryPool, scheduler, PipeScheduler.ThreadPool, maxWriteBufferSize, maxWriteBufferSize / 2, useSynchronizationContext: false);

            var pair = DuplexPipe.CreateConnectionPair(inputOptions, outputOptions);

            this.Transport = pair.Transport;
            this.Application = pair.Application;
        }

        private void Run()
        {
            this.Id = IdGeneratorHelper.GetNextId();
            
            _processingTask = StartAsync();
        }

        private async Task StartAsync()
        {
            try
            {
                var receiveTask = DoReceive();
                var sendTask = DoSend();

                await receiveTask;
                await sendTask;

                receiver.Dispose();
                sender.Dispose();
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Unexpected exception in {nameof(TcpClient)}.{nameof(StartAsync)}.");
                sb.AppendLine(ex.ToString());
                this.onException?.Invoke(new Exception(sb.ToString()));
            }
        }

        private async Task DoReceive()
        {            
            try
            {
                await ProcessReceives();
            }
            catch(Exception ex)
            {
                if (SocketErrorHelper.IsSocketDisabledError(ex) || ex is RemoteSocketClosedException)
                {
                    await this.DisposeAsync();
                }
                else
                {
                    this.onException?.Invoke(ex);
                }
            }
        }
        
        private async Task ProcessReceives()
        {
            var input = Input;
            while (true)
            {
                if (waitForData) await receiver.WaitForDataAsync();
                
                var buffer = input.GetMemory(minAllocBufferSize);
                var bytesReceived = await receiver.ReceiveAsync(buffer);
                
                if (bytesReceived == 0) 
                    throw new RemoteSocketClosedException();

                input.Advance(bytesReceived);
                var result = await input.FlushAsync();
                if (result.IsCompleted || result.IsCanceled) break;
            }
        }
        
        private async Task DoSend()
        {
            try
            {
                await ProcessSends();
            }
            catch(Exception ex)
            {
                if (SocketErrorHelper.IsSocketDisabledError(ex) || ex is RemoteSocketClosedException)
                {
                    await this.DisposeAsync();
                }
                else
                {
                    this.onException?.Invoke(ex);
                }
            }
        }
        
        private async Task ProcessSends()
        {
            var output = Output;
            while (true)
            {
                var result = await output.ReadAsync();
                if (result.IsCanceled) break;
                var buffer = result.Buffer;
                var end = buffer.End;
                var isCompleted = result.IsCompleted;
                if (!buffer.IsEmpty)
                {
                    var bytesReceived = await sender.SendAsync(buffer);
                    if (bytesReceived == 0)
                        throw new RemoteSocketClosedException();
                }

                output.AdvanceTo(end);
                if (isCompleted) break;
            }
        }
        
        public override async ValueTask DisposeAsync()
        {
            if(this.isDisposed)
            {
                return;
            }

            this.isDisposed = true;
            if(this.onClosed != null)
            {
                this.onClosed(this);
                this.onClosed = null;
            }

            this.Id = string.Empty;
            if (this.connectSocket != null)
            {
                try
                {
                    this.connectSocket.Shutdown(SocketShutdown.Both);
                }
                catch
                {
                }
                this.connectSocket.Dispose();
            }
            if (this._processingTask != null)
            {
                await this._processingTask;
            }
            
            this.Transport.Input.Complete();
            this.Transport.Output.Complete();
            
            this.Application.Input.Complete();
            this.Application.Output.Complete();
            
            await base.DisposeAsync();
        }
    }
}