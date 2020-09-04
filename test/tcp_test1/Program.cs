using System;
using System.Net;
using System.Threading.Tasks;
using Letter.Tcp;
using Letter.Tcp.Box;

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

            for (int i = 0; i < 5000; i++)
            {
                var client = TcpNetworkFactory.Client();
                client.ConfigureOptions((options) =>
                {
                    
                });
                client.Build();
                await client.ConnectAsync(address);

            }

            Console.ReadKey();
        }

        private static int num;
        private static async void OnRun()
        {
            while (true)
            {
                var c = await server.AcceptAsync();
                num ++;
                Console.WriteLine("lianjie        " + num);
            }
        }
    }
}