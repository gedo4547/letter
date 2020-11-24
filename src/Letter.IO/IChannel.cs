using System.Threading.Tasks;

namespace Letter.IO
{
    public interface IChannel
    {
        Task StopAsync();
    }
}