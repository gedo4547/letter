#if NETSTANDARD2_0
using System.Threading.Tasks;

namespace Letter
{
    public interface IAsyncDisposable
    {
        ValueTask DisposeAsync();
    }
}
#endif
