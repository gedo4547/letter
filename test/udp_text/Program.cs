﻿using System;
using System.Threading.Tasks;
using Letter.Udp;

namespace udp_text
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IUdpBootstrap bootstrap = UdpFactory.Bootstrap();
            bootstrap.ConfigurationOptions((options =>
            {
                
            }));
            bootstrap.AddChannelFilter(() => { return new UdpFilter_1();});
            bootstrap.AddChannelFilter<UdpFilter_2>();
            
            IUdpChannel channel = await bootstrap.BuildAsync();
            
            await channel.StartAsync(null);
            await channel.DisposeAsync();
            
            // var udp = UdpFactory.Create();
            // udp.ConfigurationNetwork(network =>
            // {
            //     
            // });
            
            // var udp1 = UdpFactory.Client();
            // udp1.ConfigurationOptions((options =>
            // {
            //     
            // }));
            //
            // udp1.AddFilter(() => { return new UdpFilter_1("uuuuuu_____11111111111");});
            // udp1.Build();
            // udp1.StartAsync(new IPEndPoint(IPAddress.Loopback, 20001));
            //
            //
            //
            // var udp2 = UdpFactory.Client();
            // udp2.ConfigurationOptions((options =>
            // {
            //     
            // }));
            //
            // udp2.AddFilter(() => { return new UdpFilter_2("uuuuuu_____222222222222");});
            // udp2.Build();
            // udp2.StartAsync(new IPEndPoint(IPAddress.Loopback, 20002));


            Console.ReadKey();
        }
    }
}