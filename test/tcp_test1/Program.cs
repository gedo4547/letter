using System.Net;
using System.Threading.Tasks;
using Letter.Tcp;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace tcp_test1
{
    class Program
    {
        private static bool isUseSsl = false;
        private static IPEndPoint address = new IPEndPoint(IPAddress.Loopback, 20001);
        
        static async Task Main(string[] args)
        {
            var cert = new X509Certificate2(Path.Combine(AppContext.BaseDirectory, "dotnetty.com.pfx"), "password");
           

            var server_bootstrap = TcpFactory.ServerBootstrap();
            server_bootstrap.ConfigurationOptions(options=>
            {
            });

            if (isUseSsl)
            {
                server_bootstrap.ConfigurationSsl(
                    new SslServerOptions(cert, false, false),
                    (stream)=>
                    {
                        return new SslStream(stream, true, (sender, certificate, chain, sslPolicyErrors) => true);
                    }
                );
            }
            
           

            server_bootstrap.AddChannelFilter<DefaultFixedHeaderChannelFilter>();
            server_bootstrap.AddChannelFilter<TcpTestFilter_Server>();

            var s_channel = await server_bootstrap.BuildAsync(); 
            await s_channel.StartAsync(address);
            
            
            var client_bootstrap = TcpFactory.ClientBootstrap();
            client_bootstrap.ConfigurationOptions(options =>
            {
            });

            if (isUseSsl)
            {
                string targetHost = cert.GetNameInfo(X509NameType.DnsName, false);
                client_bootstrap.ConfigurationSsl(
                    new SslClientOptions(targetHost),
                    (stream)=>
                    {
                        return new SslStream(stream, true, (sender, certificate, chain, sslPolicyErrors) => true);
                    }
                );
            }
           

            client_bootstrap.AddChannelFilter<DefaultFixedHeaderChannelFilter>();
            client_bootstrap.AddChannelFilter<TcpTestFilter_Client>();

            var c_channel = await client_bootstrap.BuildAsync();
            await c_channel.StartAsync(address);
            
            Console.ReadKey();
        }
    }
}