using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;

namespace Letter
{
    public interface ISession
    {
        string Id { get; }

        EndPoint LoaclAddress { get; }
        
        EndPoint RemoteAddress { get; }
        
        MemoryPool<byte> MemoryPool { get; }

        PipeScheduler Scheduler { get; }

        Task CloseAsync();
    }
}