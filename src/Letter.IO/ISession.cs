using System.Buffers;
using System.Buffers.Binary;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;

namespace Letter.IO
{
    public interface ISession
    {
        string Id { get; }

        BinaryOrder Order { get; }

        EndPoint LocalAddress { get; }
        
        MemoryPool<byte> MemoryPool { get; }

        PipeScheduler Scheduler { get; }
        
        Task CloseAsync();
    }
}