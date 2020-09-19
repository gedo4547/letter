using System.Net;
using System.Threading.Tasks;
using Letter.Tcp;
using System;

namespace tcp_test1
{
    class Program
    {
        private static IPEndPoint address = new IPEndPoint(IPAddress.Loopback, 20001);
        
        static async Task Main(string[] args)
        {
            var server_bootstrap = TcpFactory.ServerBootstrap();
            server_bootstrap.ConfigurationOptions(options=>
            {
            });
            
            server_bootstrap.AddChannelFilter<DefaultFixedHeaderChannelFilter>();
            server_bootstrap.AddChannelFilter<TcpTestFilter_Server>();


            var s_channel = await server_bootstrap.BuildAsync(); 
            await s_channel.StartAsync(address);
            
            
            var client_bootstrap = TcpFactory.ClientBootstrap();
            client_bootstrap.ConfigurationOptions(options =>
            {
            });
            client_bootstrap.AddChannelFilter<DefaultFixedHeaderChannelFilter>();
            client_bootstrap.AddChannelFilter<TcpTestFilter_Client>();

            var c_channel = await client_bootstrap.BuildAsync();
            await c_channel.StartAsync(address);
            


            
            Console.ReadKey();
        }

        private static void OnRun(object obj)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (System.Exception ex)
            {
                
                throw ex;
            }
            
        }
    }
}