using System;
using System.Net;
using System.Threading.Tasks;
using Letter.Udp;

namespace udp_text
{
    class Program
    {
        public static IPEndPoint s_p = new IPEndPoint(IPAddress.Loopback, 20001);
        public static IPEndPoint c_p = new IPEndPoint(IPAddress.Loopback, 20002);
        
        static async Task Main(string[] args)
        {
            IUdpBootstrap bootstrap = UdpFactory.Bootstrap();
            bootstrap.ConfigurationOptions((options =>
            {
                
            }));
            
            bootstrap.ConfigurationFilter((pipeline) =>
            {
                pipeline.Add(new UdpFilter_2());
            });
            
            IUdpChannel s_channel = await bootstrap.BuildAsync();
            await s_channel.StartAsync(s_p);
            
            IUdpChannel c_channel = await bootstrap.BuildAsync();
            await c_channel.StartAsync(c_p);
            
            int num = 0;
            while (true)
            {
                string str = Console.ReadLine();
                if (str == "send")
                {
                    for (int i = 0; i < 100; i++)
                    {
                        num++;
                        var bytes = System.Text.Encoding.UTF8.GetBytes("你好" + num);
                        M.session.Write(Program.s_p, bytes);
                        await M.session.FlushAsync();
                    }
                }
                else if(str == "c")
                {
                    await M.session.DisposeAsync();
                }
            }
        }
    }
}