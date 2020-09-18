using System.Threading.Tasks;

namespace Letter.Tcp
{
    interface IInternalTcpSession : ITcpSession
    {
        Task StartAsync();
    }
}