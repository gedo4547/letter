using System.Net;
using System.Threading.Tasks;
using Letter.Kcp;

namespace kcp_test
{
    class Program
    {
        public static EndPoint s_address = new IPEndPoint(IPAddress.Loopback, 20001);
        private static EndPoint c_address = new IPEndPoint(IPAddress.Loopback, 20002);
        
        
        static async Task Main(string[] args)
        {
            KcpDefaultScheduler thread = new KcpDefaultScheduler();
            KcpHelpr.SettingKcpBinaryOrder(System.Buffers.Binary.BinaryOrder.BigEndian);
            
            var bootstrap = KcpFactory.Bootstrap();
            bootstrap.ConfigurationGlobalThread(thread);
            bootstrap.ConfigurationGlobalOptions(options => { });
            await bootstrap.BuildAsync();
            
            //server
            var s_channel = await bootstrap.CreateChannelAsync();
            s_channel.ConfigurationSelfFilter((pipeline) =>
            {
                pipeline.Add(new KcpFilter_S("server"));
            });
            var s_controller = s_channel.BindSelfController(new KcpController());
            await s_channel.BindAsync(s_address);

            //client
            var c_channel = await bootstrap.CreateChannelAsync();
            c_channel.ConfigurationSelfFilter((pipeline) =>
            {
                pipeline.Add(new KcpFilter_C("client"));
            });
            var c_controller = c_channel.BindSelfController(new KcpController());
            await c_channel.BindAsync(c_address);
            
            s_controller.Connect(1, c_address);
            c_controller.Connect(1, s_address);

            while (true)
            {
                string str = System.Console.ReadLine();
                if(str == "s")
                {
                    break;
                }
            }
            
            thread.Stop();
        }
    }
}