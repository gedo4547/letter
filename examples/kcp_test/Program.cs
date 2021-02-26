using System.Net;
using System.Threading.Tasks;
using common;
using Letter.Kcp;

namespace kcp_test
{
    class Program
    {
        public static EndPoint s_address = new IPEndPoint(IPAddress.Loopback, 20001);
        private static EndPoint c_address = new IPEndPoint(IPAddress.Loopback, 20002);
        
        
        static async Task Main(string[] args)
        {
            //_ = new PerformanceCounterListener();

            //System.Array.Copy
            KcpDefaultScheduler thread = new KcpDefaultScheduler();
            
            var bootstrap = KcpFactory.Bootstrap();
            bootstrap.ConfigurationGlobalThread(thread);
            bootstrap.ConfigurationGlobalOptions(options => { });
            await bootstrap.BuildAsync();
            
            //server
            IKcpChannel s_channel = await bootstrap.CreateChannelAsync();
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

            for (int i = 0; i < 1000; i++)
            {
                s_controller.Connect((uint)i, c_address);
                c_controller.Connect((uint)i, s_address);
            }

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes("你好");
            while (true)
            {
                string str = System.Console.ReadLine();
                if(str == "s")
                {
                    break;
                }

                if (str == "")
                {
                    var sessions = M.sessions;
                    for (int i = 0; i < sessions.Count; i++)
                    {
                        var session = sessions[i];
                        session.SendUnreliableAsync(bytes);
                        //session.SendSafeAsync(bytes);
                    }
                }

            }
            
            thread.Stop();
        }
    }
}