using System.Net;
using System.Threading.Tasks;
using Letter.Kcp;

namespace kcp_test
{
    class Program
    {
        private static EndPoint s_address = new IPEndPoint(IPAddress.Loopback, 20001);
        private static EndPoint c_address = new IPEndPoint(IPAddress.Loopback, 20002);
        
        
        static async Task Main(string[] args)
        {
            KcpDefaultThread thread = new KcpDefaultThread();
            thread.Start();
            
            var bootstrap = KcpFactory.Bootstrap();
            bootstrap.ConfigurationGlobalThread(thread);
            bootstrap.ConfigurationGlobalOptions(options => { });
            await bootstrap.BuildAsync();
            
            var s_channel = await bootstrap.CreateAsync();
            s_channel.ConfigurationSelfFilter((pipeline) =>
            {
                pipeline.Add(new KcpFilter_S("server"));
            });
            await s_channel.BindAsync(s_address);
            s_channel.Connect(1, c_address);
            
            
            var c_channel = await bootstrap.CreateAsync();
            c_channel.ConfigurationSelfFilter((pipeline) =>
            {
                pipeline.Add(new KcpFilter_C("client"));
            });
            await c_channel.BindAsync(c_address);
            c_channel.Connect(1, s_address);
            

            while (true)
            {
                System.Threading.Thread.Sleep(1);
            }
            
           
            
            thread.Stop();
        }
    }
}