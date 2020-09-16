using System.IO.Pipelines;

namespace System.Net.Sockets
{
    public abstract class SocketBase : IDisposable
    {
        protected SocketBase(Socket socket, PipeScheduler scheduler)
        {
            _socket = socket;
            _awaitableEventArgs = new SocketAwaitableEventArgs(scheduler);
        }
        
        protected readonly Socket _socket;
        protected readonly SocketAwaitableEventArgs _awaitableEventArgs;

        public EndPoint RemoteAddress
        {
            get { return _awaitableEventArgs.RemoteEndPoint; }
        }

        public EndPoint LocalAddress
        {
            get { return _socket.LocalEndPoint; }
        }

        public void Dispose() => _awaitableEventArgs.Dispose();
    }
}