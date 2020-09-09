using System.Net;

namespace Letter.Udp
{
    public interface IUdpClient : IClient<UdpClientOptions>
    {
        void Bind(EndPoint point);
    }
}