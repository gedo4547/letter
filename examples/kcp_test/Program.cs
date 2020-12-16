using System.Net;
using System.Threading.Tasks;
using Letter.Kcp;

namespace kcp_test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            KcpDefaultScheduler scheduler = new KcpDefaultScheduler();
            scheduler.Start();
            
            var bootstrap = KcpFactory.Bootstrap();
            bootstrap.ConfigurationGlobalOptions(options => { });
            bootstrap.ConfigurationGlobalScheduler(scheduler);
            await bootstrap.BuildAsync();
            
            var channel = await bootstrap.CreateAsync();
            await channel.BindAsync(new IPEndPoint(IPAddress.Any, 20001));
            
            var session = channel.CreateSession(new IPEndPoint(IPAddress.Any, 20001));

            
            
            scheduler.Stop();
        }
    }
}