using System.Buffers;
using System.Threading.Tasks;

namespace Letter.Tcp
{
    public interface ITcpSession : ISession
    {
        Task WriteAsync(object obj);
        Task WriteAsync(ref ReadOnlySequence<byte> sequence);
    }
}