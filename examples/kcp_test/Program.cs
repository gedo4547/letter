using System.Net;
using System.Threading.Tasks;
using Letter.Kcp;

namespace kcp_test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            KcpDefaultThread thread = new KcpDefaultThread();
            thread.Start();
            
            var bootstrap = KcpFactory.Bootstrap();
            bootstrap.ConfigurationGlobalThread(thread);
            bootstrap.ConfigurationGlobalOptions(options => { });
            bootstrap.ConfigurationGlobalFilter((pipeline) =>
            {
                
            });
            await bootstrap.BuildAsync();
            
            var channel = await bootstrap.CreateAsync();
            // channel.ConfigurationRouter();
            await channel.BindAsync(new IPEndPoint(IPAddress.Any, 20001));
            
            channel.Connect(1, new IPEndPoint(IPAddress.Any, 20001));

            
            
            thread.Stop();
        }
    }
}