using System.Threading.Tasks;

namespace Letter.Tcp.Box
{
    public interface ITcpSession : Bootstrap.ISession
    {
        Task WriteAsync(object o);
    }
}