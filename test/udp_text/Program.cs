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

            await bootstrap.BuildAsync();
            
            IUdpChannel s_channel = await bootstrap.CreateAsync();
            await s_channel.StartAsync(s_p);
            
            IUdpChannel c_channel = await bootstrap.CreateAsync();
            await c_channel.StartAsync(c_p);
            int count = 0;
            while (true)
            {
                string str = Console.ReadLine();
                if (str == "send")
                {
                    count++;
                    for (int i = 1; i <= 100; i++)
                    {
                        for (int j = 1; j <= 5; j++)
                        {
                            string com = $"{count}____{i}______{j}";
                            var bytes = System.Text.Encoding.UTF8.GetBytes(com);
                            M.session.Write(Program.s_p, bytes);
                        }

                        await M.session.FlushAsync();
                    }
                }
                else if(str == "c")
                {
                    await M.session.CloseAsync();
                }
                else if(str == "s")
                {
                    return;
                }
            }
        }
    }
}