using System;
using System.Net;
using System.Net.Security;
using System.Threading.Tasks;
using Letter.Tcp;

namespace tcp_server
{
    class Program
    {
        private static IPEndPoint address = new IPEndPoint(IPAddress.Loopback, 20001);
        
        static async Task Main(string[] args)
        {
            Console.Title = "server";
            
            ITcpServerBootstrap server_bootstrap = TcpFactory.ServerBootstrap();
            server_bootstrap.ConfigurationOptions(options => { });
            server_bootstrap.ConfigurationFilter((pipeline) =>
            {
                pipeline.Add(new DefaultFixedHeaderBytesFilter());
                pipeline.Add(new TcpServerFilter());
            });

            if (common.SocketConfig.useSsl)
            {
                server_bootstrap.ConfigurationSsl(new SslServerOptions(common.SocketConfig.cert, false, false), stream =>
                {
                    return new SslStream(stream, true, (sender, certificate, chain, sslPolicyErrors) => true);
                });
            }
            await server_bootstrap.BuildAsync();
            
            var s_channel = await server_bootstrap.CreateAsync();
            await s_channel.StartAsync(address);


            while (true)
            {
                string str = Console.ReadLine();
                if (str == "c")
                {
                    return;
                }

                if (str == "t")
                {
                    Console.WriteLine("当前客户端连接数量：" + ServerStatistics.client_count);
                }
            }
        }
    }
}