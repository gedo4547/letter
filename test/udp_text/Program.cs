using System;
using System.Diagnostics;
using System.IO.Pipelines;
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
            
            Console.ReadKey();
        }
    }
}