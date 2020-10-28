using System.Threading.Tasks;

namespace Letter.Tcp
{
    public interface ITcpSession : ISession
    {
        void Write(object o);

        Task FlushAsync();
    }
}