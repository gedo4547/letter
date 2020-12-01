using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace dotnetty_tcp_client
{
    class Program
    {
        public static Action sendCallback;

        private static DateTime startTime;
        private static IPEndPoint address = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 20001);
        private static List<ClientHandler> clients = new List<ClientHandler>();

        static async Task Main(string[] args)
        {
            Console.Title = "client";

            var group = new MultithreadEventLoopGroup();
            var bootstrap = new Bootstrap();
            bootstrap
                .Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {

                    IChannelPipeline pipeline = channel.Pipeline;
                    var client = new ClientHandler();
                    clients.Add(client);
                    //if (cert != null)
                    //{
                    //    pipeline.AddLast("tls", new TlsHandler(stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true), new ClientTlsSettings(targetHost)));
                    //}
                    //pipeline.AddLast(new LoggingHandler());
                    pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                    pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

                    pipeline.AddLast("echo", client);
                }));

            for (int i = 0; i < 1; i++)
            {
                IChannel clientChannel = await bootstrap.ConnectAsync(address);
            }

            
            System.Threading.Thread.Sleep(3 * 1000);
            Console.WriteLine("start send");
            
            startTime = DateTime.Now;
            sendCallback();

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
                    foreach (var filter in clients)
                    {
                        count += filter.count;
                    }

                    Console.WriteLine("平均每秒：" + count / totalSeconds);
                }
            }
        }
    }
}
