using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace kcp_lab_test2
{
    class Program
    {
        static KcpUnit unit1;
        static KcpUnit unit2;

        private static int num = 0;

        private static bool use_buffer = false;

        private static bool isRun = true;
        private static List<KcpUnit> units_1 = new List<KcpUnit>();
        private static List<KcpUnit> units_2 = new List<KcpUnit>();

        static void Main(string[] args)
        {
            Console.Title = "kcp_lib1111111111111111111111111111111";
            var memoryPool = SlabMemoryPoolFactory.Create(new MemoryPoolOptions(4096, 32));

            for (int i = 0; i < 5000; i++)
            {
                uint conv = (uint)i;
                var t_unit1 = new KcpUnit(conv);
                t_unit1.SetRcvEvent((ref Memory<byte> memory) => { OnRcvEvent(ref memory, 1); });
                t_unit1.SetSndEvent((ref Memory<byte> memory) => { OnSndEvent(ref memory, 1); });
                units_1.Add(t_unit1);

                var t_unit2 = new KcpUnit(conv);
                t_unit2.SetRcvEvent((ref Memory<byte> memory) => { OnRcvEvent(ref memory, 2); });
                t_unit2.SetSndEvent((ref Memory<byte> memory) => { OnSndEvent(ref memory, 2); });
                units_2.Add(t_unit2);
            }

            Thread thread = new Thread(() =>
            {

                while (isRun)
                {
                    Thread.Sleep(5);

                    int units1Count = units_1.Count;
                    for (int i = 0; i < units1Count; i++)
                    {
                        units_1[i].Update();
                    }

                    int units2Count = units_2.Count;
                    for (int i = 0; i < units2Count; i++)
                    {
                        units_2[i].Update();
                    }
                }

            });
            thread.Start();


            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(common.Message.data_1024);
            while (true)
            {
                // System.Threading.Thread.Sleep(1);
                string str = Console.ReadLine();

                if (int.TryParse(str, out var length))
                {
                    for (int i = 0; i < length; i++)
                    {
                        Interlocked.Increment(ref num);
                        if (use_buffer)
                        {
                            unit1.Send(buffer);
                        }
                        else
                        {
                            unit1.Send(System.Text.Encoding.UTF8.GetBytes($"nihao {i}"));
                        }
                    }
                }
                else if (string.IsNullOrEmpty(str))
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        Interlocked.Increment(ref num);
                        if (use_buffer)
                        {
                            unit1.Send(buffer);
                        }
                        else
                        {
                            unit1.Send(System.Text.Encoding.UTF8.GetBytes($"nihao {i}"));
                        }
                    }
                }
                else if (str == "s")
                {
                    isRun = false;
                    return;
                }
                else if (str == "d")
                {
                    unit1.Debug();
                    //unit2.Debug();
                }

            }
        }

        private static void OnRcvEvent(ref Memory<byte> memory, int type)
        {
            Console.WriteLine($"{type}>rcv>>" + Encoding.UTF8.GetString(memory.Span));
        }

        private static void OnSndEvent(ref Memory<byte> memory, int type)
        {
            //Console.WriteLine($"{type}>snd>>" + sequence.Length);
            byte[] bytes = new byte[memory.Length];
            memory.CopyTo(bytes);

            if (type == 1)
            {
                unit2.Recv(bytes);
            }
            else if (type == 2)
            {
                unit1.Recv(bytes);
            }
        }
    }
}