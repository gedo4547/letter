using System.Net;
using System.Threading.Tasks;

namespace tcp_test1
{
    class Program
    {
        // private static byte[] symbol = {(byte) '\n'};
        private static IPEndPoint address = new IPEndPoint(IPAddress.Loopback, 20001);
        //
        static async Task Main(string[] args)
        {
            var server_bootstrap = Letter.Tcp.TcpFactory.ServerBootstrap();
            server_bootstrap.ConfigurationOptions(options=>
            {
            });
            //
            //
            // server_bootstrap.AddChannelFilter<>
            var s_channel = await server_bootstrap.BuildAsync(); 
            await s_channel.StartAsync(address);
            
            
        //     var server = TcpFactory.ServerBootstrap();
        //     server.ConfigurationOptions((options) =>
        //     {
        //         
        //     });
        //     server.AddFilter(() => { return new DefaultFixedSymbolFilter(symbol);});
        //     server.AddFilter(() => { return new TcpTestFilter_Server("服务器");});
        //     await server.StartAsync(address);
        //     
        //     var client = TcpFactory.ClientBootstrap();
        //     client.ConfigurationOptions((options) =>
        //     {
        //         
        //     });
        //     client.AddFilter(() => { return new DefaultFixedSymbolFilter(symbol);});
        //     client.AddFilter(() => { return new TcpTestFilter_Client("客户端");});
        //     await client.StartAsync(address);
        //     
            
            // Console.ReadKey();
        }

      
    }
}