using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Net;
using System.Threading.Tasks;

namespace dotnetty_tcp_client
{
    class Program
    {
        private static IPEndPoint address = new IPEndPoint(IPAddress.Loopback, 20001);

        static async Task Main(string[] args)
        {
            var group = new MultithreadEventLoopGroup();
            var bootstrap = new Bootstrap();
            bootstrap
                .Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {

                    IChannelPipeline pipeline = channel.Pipeline;

                    //if (cert != null)
                    //{
                    //    pipeline.AddLast("tls", new TlsHandler(stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true), new ClientTlsSettings(targetHost)));
                    //}
                    //pipeline.AddLast(new LoggingHandler());
                    pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                    pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

                    pipeline.AddLast("echo", new ClientHandler());
                }));

            for (int i = 0; i < 5000; i++)
            {
                IChannel clientChannel = await bootstrap.ConnectAsync(address);
            }
        }
    }
}
