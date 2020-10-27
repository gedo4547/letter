using System.Threading.Tasks;

namespace Letter.Tcp.Box
{
    interface ITcpInternalSession : ITcpSession
    {
        Task StartAsync();
    }
}