using System.Net;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public interface ITcpServerChannel : IChannel
    {
        Task BindAsync(EndPoint address);
    }
}