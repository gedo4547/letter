using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Letter.Tcp;

namespace tcp_test1
{
    class Program
    {
        private static ITcpServer server;
        private static IPEndPoint address = new IPEndPoint(IPAddress.Loopback, 20001);
        
        static async Task Main(string[] args)
        {
            var server = TcpFactory.ServerBootstrap();
            server.ConfigurationOptions((options) =>
            {
                
            });
            server.AddChannel(() => { return new TcpTestChannel_1("服务器");});
            server.AddChannel(() => { return new TcpTestChannel_2("服务器");});
            await server.StartAsync(new IPEndPoint(IPAddress.Loopback, 20001));



            var client = TcpFactory.ClientBootstrap();
            client.ConfigurationOptions((options) =>
            {
                
            });
            client.AddChannel(() => { return new TcpTestChannel_1("客户端");});
            client.AddChannel(() => { return new TcpTestChannel_2("客户端");});
            await client.StartAsync(new IPEndPoint(IPAddress.Loopback, 20001));
            
            
            Console.ReadKey();
        }

      
    }
}