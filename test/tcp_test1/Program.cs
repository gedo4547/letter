using System.Net;
using System.Threading.Tasks;
using Letter.Tcp;

namespace tcp_test1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ITcpListener listener = TcpNetworkFactory.Listener();
            listener.ConfigureLogger(null);
            listener.ConfigureOptions(options => { });
            listener.Build();
            listener.Bind(new IPEndPoint(IPAddress.Loopback, 20001));
            
            while (true)
            {
                var session = await listener.AcceptAsync();
                if (session == null) break;
            }
        }
    }
}