using Letter.Tcp;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using tcp_test1;

namespace tcp_s
{
    class Program
    {

        private static bool isUseSsl = false;
        private static IPEndPoint address = new IPEndPoint(IPAddress.Loopback, 20001);
        private static X509Certificate2 cert = new X509Certificate2(Path.Combine(AppContext.BaseDirectory, "dotnetty.com.pfx"), "password");

        static async Task Main(string[] args)
        {
            var server_bootstrap = TcpFactory.ServerBootstrap();
            server_bootstrap.ConfigurationOptions(options =>
            {

            });

            server_bootstrap.ConfigurationFilter((pipeline) =>
            {
                pipeline.Add(new DefaultFixedHeaderBytesFilter());
                pipeline.Add(new TcpTestFilter_Server());
            });

            if (isUseSsl)
            {
                server_bootstrap.ConfigurationSsl(new SslServerOptions(cert, false, false), stream =>
                {
                    return new SslStream(stream, true, (sender, certificate, chain, sslPolicyErrors) => true);
                });
            }


            var s_channel = await server_bootstrap.BuildAsync();
            await s_channel.StartAsync(address);


            Console.ReadKey();
        }
    }
}
