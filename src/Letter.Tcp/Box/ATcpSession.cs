using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    abstract class ATcpSession
    {
        public ATcpSession(Socket socket, Box.ATcpOptions options, PipeScheduler scheduler, MemoryPool<byte> pool)
        {
            this.pool = pool;
            this.scheduler = scheduler;
            this.socket = new TcpSocket(socket, scheduler);
            
            this.SettingSocket(this.socket, options);
        }

        private TcpSocket socket;
        private MemoryPool<byte> pool;
        private PipeScheduler scheduler;

        public abstract Task StartAsync();


        private void SettingSocket(TcpSocket socket, Box.ATcpOptions options)
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


    }
}