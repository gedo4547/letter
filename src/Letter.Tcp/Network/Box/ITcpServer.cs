using Letter.Box;

namespace Letter.Tcp.Box
{
    public interface ITcpServer : IServer<TcpListenerOptions, TcpConnectorOptions, ITcpClient>
    {
        
    }
}