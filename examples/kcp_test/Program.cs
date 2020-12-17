using System.Net;
using System.Threading.Tasks;
using Letter.Kcp;

namespace kcp_test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            KcpDefaultThread scheduler = new KcpDefaultThread();
            scheduler.Start();
            
            var bootstrap = KcpFactory.Bootstrap();
            bootstrap.ConfigurationGlobalOptions(options => { });
            bootstrap.ConfigurationGlobalThread(scheduler);
            await bootstrap.BuildAsync();
            
            var channel = await bootstrap.CreateAsync();
            await channel.BindAsync(new IPEndPoint(IPAddress.Any, 20001));
            
            channel.Connect(1, new IPEndPoint(IPAddress.Any, 20001));

            
            
            scheduler.Stop();
        }
    }
}