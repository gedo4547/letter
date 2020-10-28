using System.Threading.Tasks;

namespace Letter.Tcp
{
    public interface ITcpSession : ISession
    {
        Task WriteAsync(object o);
    }
}