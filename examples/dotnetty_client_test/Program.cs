using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace dotnetty_client_test
{
    class Program
    {
        private static IPEndPoint _port = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 20001);
        private static MultithreadEventLoopGroup group = new MultithreadEventLoopGroup();
        
        private static DateTime startTime;
        public static Action startSend = null;
        private static List<ClientChannelHandler> _clientFilters = new List<ClientChannelHandler>();

        static async Task Main(string[] args)
        {
            Buffer.messageBuffer = Unpooled.Buffer(common.SocketConfig.message.Length);
            Buffer.messageBuffer.WriteBytes(common.SocketConfig.message);
            
            var bootstrap = new Bootstrap()
                .Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    
                    pipeline.AddLast(new LengthFieldPrepender(2));
                    pipeline.AddLast(new LengthFieldBasedFrameDecoder(ByteOrder.BigEndian, ushort.MaxValue, 0, 2, 0, 2, true));
                    
                    var client = new ClientChannelHandler();
                    _clientFilters.Add(client);

                    pipeline.AddLast(client);
                }));

            for (int i = 0; i < 5000; i++)
            {
                await bootstrap.ConnectAsync(_port);
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
                    
                    Console.WriteLine($"当前客户端数量：{_clientFilters.Count}  平均每秒：" + count / totalSeconds);
                }
            }
        }
    }
}