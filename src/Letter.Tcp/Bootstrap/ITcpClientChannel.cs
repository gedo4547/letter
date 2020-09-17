using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public interface ITcpClientChannel : IChannel
    {
        Task ConnectAsync(EndPoint address);
    }
}