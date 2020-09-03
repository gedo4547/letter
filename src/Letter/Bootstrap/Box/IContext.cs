using System.Buffers;
using System.Net;
using System.Threading.Tasks;

namespace Letter.Box
{
    public interface IContext
    {
        string Id { get; }

        EndPoint LoaclAddress { get; }
        
        EndPoint RemoteAddress { get; }
        
        MemoryPool<byte> MemoryPool { get; }
        
        Task WriteAsync(object o);
        
        Task WriteAsync(byte[] bytes, int offset, int count);

        Task CloseAsync();
    }
}