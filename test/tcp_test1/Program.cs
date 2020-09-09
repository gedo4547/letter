using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Letter.Tcp;

namespace tcp_test1
{
    class Program
    {
        private static byte[] symbol = {(byte) '\n'};
        private static IPEndPoint address = new IPEndPoint(IPAddress.Loopback, 20001);
        
        static async Task Main(string[] args)
        {
            var server = TcpFactory.ServerBootstrap();
            server.ConfigurationOptions((options) =>
            {
                
            });
            server.AddChannel(() => { return new DefaultFixedSymbolChannel(symbol);});
            server.AddChannel(() => { return new TcpTestChannel_Server("服务器");});
            await server.StartAsync(address);



            var client = TcpFactory.ClientBootstrap();
            client.ConfigurationOptions((options) =>
            {
                
            });
            client.AddChannel(() => { return new DefaultFixedSymbolChannel(symbol);});
            client.AddChannel(() => { return new TcpTestChannel_Client("客户端");});
            await client.StartAsync(address);
            
            
            Console.ReadKey();
        }

      
    }
}