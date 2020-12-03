using System.Net;
using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Tcp
{
    public interface ITcpSession : ISession
    {
        EndPoint RemoteAddress { get; }

        void Write(object o);

        ValueTask FlushAsync();
    }
}