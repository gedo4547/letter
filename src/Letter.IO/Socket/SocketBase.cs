using System.IO.Pipelines;

namespace System.Net.Sockets
{
    public abstract class SocketBase : IDisposable
    {
        protected readonly Socket _socket;
        protected readonly SocketAwaitableEventArgs _awaitableEventArgs;

        protected SocketBase(Socket socket, PipeScheduler scheduler)
        {
            _socket = socket;
            _awaitableEventArgs = new SocketAwaitableEventArgs(scheduler);
        }

        public void Dispose() => _awaitableEventArgs.Dispose();
    }
}