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
            server = TcpNetworkFactory.Server();
            server.ConfigureOptions((options) =>
            {
                
            });
            
            server.Build();
            server.Bind(address);
            Task.Run(OnRun);

            System.Threading.Thread.Sleep(2 * 1000);
            Console.WriteLine("开始连接");
            List<ITcpClient> clients = new List<ITcpClient>();
            for (int i = 0; i < 1; i++)
            {
                var client = TcpNetworkFactory.Client();
                client.ConfigureOptions((options) =>
                {
                    
                });
                client.Build();
                await client.ConnectAsync(address);
                clients.Add(client);
            }

            
            System.Threading.Thread.Sleep(5 * 1000);
            Console.WriteLine("开始关闭");
            for (int i = 0; i < clients.Count; i++)
            {
                await clients[i].CloseAsync();
            }
            
            
            Console.ReadKey();
        }

        private static int num;
        private static async void OnRun()
        {
            while (true)
            {
                var c = await server.AcceptAsync();
                c.AddExceptionListener(OnServerClientException);
                
                num ++;
                Console.WriteLine("lianjie        " + num);
            }
        }

        private static void OnServerClientException(Exception ex)
        {
            Console.WriteLine(">OnServerClientException>>>"+ ex.ToString());
        }
    }
}