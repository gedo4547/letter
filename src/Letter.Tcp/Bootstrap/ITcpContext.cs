using System.Threading.Tasks;

namespace Letter.Tcp
{
    public interface ITcpContext : IContext
    {
        Task WriteAsync(object o);
        Task WriteAsync(byte[] buffer, int offset, int count);
    }
}