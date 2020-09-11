using System;
using System.Buffers;
using System.Net;
using System.Threading.Tasks;

namespace Letter
{
    public interface IContext : IAsyncDisposable
    {
        string Id { get; }

        EndPoint LoaclAddress { get; }
        
        EndPoint RemoteAddress { get; }
        
        MemoryPool<byte> MemoryPool { get; }

        Task CloseAsync();
    }
}