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
        private static int count;
        public static IPEndPoint s_p = new IPEndPoint(IPAddress.Loopback, 20001);
        public static IPEndPoint c_p = new IPEndPoint(IPAddress.Loopback, 20002);


        static void MMMMMMMMMMMMMMMMM()
        {
            var watch = Stopwatch.StartNew();
            int num = 0;
            for (int k = 0; k < count; k++)
            {
                num++;
            }
            watch.Stop();
            Console.WriteLine($"耗费：{watch.ElapsedMilliseconds}");
        }

        static void Main(string[] args)
        {
            string str = Console.ReadLine();
            count = int.Parse(str);

            for (int i = 0; i < 5; i++)
            {
                MMMMMMMMMMMMMMMMM();
                Console.WriteLine($"--------------------------------------------------------");
                try
                {
                    MMMMMMMMMMMMMMMMM();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                Console.WriteLine($"===========================================");
            }
            
           
           
            
            // MemorySegment segment3 = new MemorySegment(null);
            //
            // MemorySegment segment2 = new MemorySegment(null);
            // segment2.SetNext(segment3);
            //
            // MemorySegment segment1 = new MemorySegment(null);
            // segment1.SetNext(segment2);
            //
            // SegmentSequence sequence = new SegmentSequence(segment1, segment3);
            // foreach (var item in sequence)
            // {
            //     Console.WriteLine(">>>>>>>");
            // }
            //
            //
            // string str = "";
            // SegmentSequence sequence = new SegmentSequence();



            // IUdpBootstrap bootstrap = UdpFactory.Bootstrap();
            // bootstrap.ConfigurationOptions(options =>
            // {
            //     
            // });
            // bootstrap.AddChannelFilter<UdpFilter_2>();
            //
            // IUdpChannel s_channel = await bootstrap.BuildAsync();
            // await s_channel.StartAsync(s_p);
            //
            // IUdpChannel c_channel = await bootstrap.BuildAsync();
            // await c_channel.StartAsync(c_p);
            //
            // Console.ReadKey();
        }
    }
}