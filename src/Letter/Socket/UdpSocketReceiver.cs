using System.IO.Pipelines;

namespace System.Net.Sockets
{
    public class UdpSocketReceiver : SocketBase
    {
        public UdpSocketReceiver(Socket socket, PipeScheduler scheduler) : base(socket, scheduler)
        {
        }
        
        public SocketAwaitableEventArgs ReceiveAsync(EndPoint loacl, Memory<byte> buffer)
        {
            _awaitableEventArgs.SetBuffer(buffer);
            _awaitableEventArgs.RemoteEndPoint = loacl;
            if (!_socket.ReceiveFromAsync(_awaitableEventArgs))
            {
                _awaitableEventArgs.Complete();
            }

            return _awaitableEventArgs;
        }
    }
}