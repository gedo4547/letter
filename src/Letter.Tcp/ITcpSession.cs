using System.Threading.Tasks;
using Letter.IO;

namespace Letter.Tcp
{
    public interface ITcpSession : ISession
    {
        void Write(object o);

        Task FlushAsync();
    }
}