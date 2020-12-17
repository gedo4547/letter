using System.Net;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Kcp
{
    public interface IKcpSession : ISession
    {
        EndPoint RemoteAddress { get; }

        // IFilterPipeline<IKcpSession> Pipeline { get; }

        void Write(object o);

        Task FlushAsync();
    }
}