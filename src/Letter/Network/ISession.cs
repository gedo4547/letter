using System.Threading.Tasks;

namespace Letter
{
    public interface ISession
    {
        string Id { get; }

        Task CloseAsync();
    }
}