using System;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace dotnetty_server_test
{
    class Program
    {
        private static IPEndPoint _port = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 20001);
        private static MultithreadEventLoopGroup parentGroup = new MultithreadEventLoopGroup();
        private static MultithreadEventLoopGroup childGroup = new MultithreadEventLoopGroup();
        
        static async Task Main(string[] args)
        {
            Buffer.messageBuffer = Unpooled.Buffer(common.SocketConfig.message.Length);
            Buffer.messageBuffer.WriteBytes(common.SocketConfig.message);
            
            var serverChannel = await new ServerBootstrap()
                .Group(parentGroup, childGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 1024)
                .Option(ChannelOption.TcpNodelay, true)
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    
                    pipeline.AddLast(new LengthFieldPrepender(2));
                    pipeline.AddLast(new LengthFieldBasedFrameDecoder(ByteOrder.BigEndian,ushort.MaxValue, 0, 2, 0, 2, true));
                    pipeline.AddLast(new ServerChannelHandler());
                }))
                .BindAsync(_port);

            
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