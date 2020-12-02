using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using common;
using Letter.Tcp;

namespace tcp_client
{
    class Program
    {
        private static DateTime startTime;
        private static IPEndPoint address = new IPEndPoint(IPAddress.Loopback, 20001);
        
        public static Action startSend = null;
        private static List<TcpClientFilter> _clientFilters = new List<TcpClientFilter>();
        
        static async Task Main(string[] args)
        {
            Console.Title = "client";
            var client_bootstrap = TcpFactory.ClientBootstrap();
            client_bootstrap.ConfigurationOptions(options => { });
            client_bootstrap.ConfigurationFilter((pipeline) =>
            {
                var filter = new TcpClientFilter();
                _clientFilters.Add(filter);
                
                pipeline.Add(new DefaultFixedHeaderBytesFilter());
                pipeline.Add(filter);
            });

            if (SocketConfig.useSsl)
            {
                client_bootstrap.ConfigurationSsl(new SslClientOptions(SocketConfig.cert.GetNameInfo(X509NameType.DnsName, false)), stream =>
                {
                    return new SslStream(stream, true, (sender, certificate, chain, sslPolicyErrors) => true);
                });
            }
            await client_bootstrap.BuildAsync();
            
            
            for (int i = 0; i < 5000; i++)
            {
                var c_channel = await client_bootstrap.CreateAsync();
                await c_channel.StartAsync(address);
            }
            
            System.Threading.Thread.Sleep(3 * 1000);
            Console.WriteLine("start send");
            
            startTime = DateTime.Now;
            startSend();
            
            while (true)
            {
                string str = Console.ReadLine();
                if (str == "c")
                {
                    return;
                }

                if (str == "t")
                {
                    var totalSeconds = (DateTime.Now - startTime).TotalSeconds;
                    long count = 0;
                    foreach (var filter in _clientFilters)
                    {
                        count += filter.count;
                    }
                    
                    Console.WriteLine("平均每秒：" + count / totalSeconds);
                }
            }
        }
    }
}