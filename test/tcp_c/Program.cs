using Letter.Tcp;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using tcp_test1;

namespace tcp_c
{
    class Program
    {

        private static bool isUseSsl = true;
        private static IPEndPoint address = new IPEndPoint(IPAddress.Loopback, 20001);
        private static X509Certificate2 cert = new X509Certificate2(Path.Combine(AppContext.BaseDirectory, "dotnetty.com.pfx"), "password");

        static async Task Main(string[] args)
        {
            var client_bootstrap = TcpFactory.ClientBootstrap();
            client_bootstrap.ConfigurationOptions((options =>
            {

            }));

            client_bootstrap.ConfigurationFilter((pipeline) =>
            {
                pipeline.Add(new DefaultFixedHeaderBytesFilter());
                pipeline.Add(new TcpTestFilter_Client());
            });

            if (isUseSsl)
            {
                client_bootstrap.ConfigurationSsl(new SslClientOptions(cert.GetNameInfo(X509NameType.DnsName, false)), stream =>
                {
                    return new SslStream(stream, true, (sender, certificate, chain, sslPolicyErrors) => true);
                });
            }

            await client_bootstrap.BuildAsync();

            var c_channel = await client_bootstrap.CreateAsync();
            await c_channel.StartAsync(address);

            int num = 0;
            while (true)
            {
                string str = Console.ReadLine();
                if (str == "send")
                {
                    num++;
                    var bytes = System.Text.Encoding.UTF8.GetBytes("你好" + num);
                    M.session.Write(bytes);
                    await M.session.FlushAsync();
                }
                else if (str == "c")
                {
                    await M.session.CloseAsync();
                }
            }

        }
    }
}
