using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Linq;

namespace kcp_lab_test
{
    readonly ref struct MyStruct
    {
        public MyStruct(Span<byte> span)
        {
            this.span = span;
        }
        private readonly Span<byte> span;
    }
    class Program
    {
        static KcpUnit unit1; 
        static KcpUnit unit2;

        private static int num = 0;

        private static bool use_buffer = false;
        private static bool isRun = true;
        private static List<KcpUnit> units_1 = new List<KcpUnit>();
        private static List<KcpUnit> units_2 = new List<KcpUnit>();

        static unsafe void Main(string[] args)
        {
            Console.Title = "kcp_lib1111111111111111111111111111111";
            Console.WriteLine(Process.GetCurrentProcess().Id);
            //Span<byte> span = stackalloc byte[32];
            //MyStruct my = new MyStruct(span);

            var memoryPool = SlabMemoryPoolFactory.Create(new MemoryPoolOptions(4096, 32));
            for (int i = 0; i < 5000; i++)
            {
                uint conv = (uint)i;
                var unit1 = new KcpUnit(conv, memoryPool);
                unit1.SetRcvEvent((ref ReadOnlySequence<byte> sequence) => { OnRcvEvent(ref sequence, 1, unit1.Conv); });
                unit1.SetSndEvent((ref ReadOnlySequence<byte> sequence) => { OnSndEvent(ref sequence, 1, unit1.Conv); });
                units_1.Add(unit1);

                var unit2 = new KcpUnit(conv, memoryPool);
                unit2.SetRcvEvent((ref ReadOnlySequence<byte> sequence) => { OnRcvEvent(ref sequence, 2, unit2.Conv); });
                unit2.SetSndEvent((ref ReadOnlySequence<byte> sequence) => { OnSndEvent(ref sequence, 2, unit2.Conv); });
                units_2.Add(unit2);
            }

            Thread thread = new Thread(() =>
            {
                while (isRun)
                {
                    Thread.Sleep(1);

                    int count1 = units_1.Count;
                    for (int  i= 0;  i< count1; i++)
                    {
                        units_1[i].Update();
                    }

                    int count2 = units_2.Count;
                    for (int i = 0; i < count2; i++)
                    {
                        units_2[i].Update();
                    }
                }
            });
            thread.Start();


            byte[] buffer = Encoding.UTF8.GetBytes(common.Message.data_1024);
            while (true)
            {
                // System.Threading.Thread.Sleep(1);
                string str = Console.ReadLine();
                Console.Clear();
                if (int.TryParse(str, out var length))
                {
                    for (int i = 0; i < length; i++)
                    {
                        Interlocked.Increment(ref num);
                        if (use_buffer)
                        {
                            int count1 = units_1.Count;
                            for (int j = 0; j < count1; j++)
                            {
                                units_1[j].Send(buffer);
                            }
                        }
                        else
                        {
                            var bytes = System.Text.Encoding.UTF8.GetBytes($"nihao {i}-----{num}");
                            int count1 = units_1.Count;
                            for (int j = 0; j < count1; j++)
                            {
                                units_1[j].Send(bytes);
                            }
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
                            int count1 = units_1.Count;
                            for (int j = 0; j < count1; j++)
                            {
                                units_1[j].Send(buffer);
                            }
                        }
                        else
                        {
                            var bytes = System.Text.Encoding.UTF8.GetBytes($"nihao {i}");
                            int count1 = units_1.Count;
                            for (int j = 0; j < count1; j++)
                            {
                                units_1[j].Send(bytes);
                            }
                        }
                    }
                }
                else if (str == "s")
                {
                    isRun = false;
                    return;
                }
                else
                {
                    //d-type-index
                    if (str.Contains("d"))
                    {
                        var arr = str.Split('-');
                        var type = int.Parse(arr[1]);
                        var index = int.Parse(arr[2]);
                        if (type == 1)
                        {
                            for (int i = index; i < units_1.Count; i++)
                            {
                                units_1[i].Debug();
                            }
                        }

                        if (type == 2)
                        {
                            for (int i = index; i < units_2.Count; i++)
                            {
                                units_2[i].Debug();
                            }
                        }
                       
                    }
                }

            }
        }
        
        private static void OnRcvEvent(ref ReadOnlySequence<byte> sequence, int type, uint conv)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in sequence)
            {
                sb.Append(Encoding.UTF8.GetString(item.Span));
            }
            
            Console.WriteLine($"{type}>{conv}>>rcv>>" + sb.ToString()+">>time:"+DateTime.Now);
        }
        
        private static void OnSndEvent(ref ReadOnlySequence<byte> sequence, int type, uint conv)
        {
            //Console.WriteLine($"{type}>snd>>" + sequence.Length);
            byte[] bytes = new byte[sequence.Length];
            sequence.CopyTo(bytes);

            if (type == 1)
            {
                units_2[(int)conv].Recv(bytes);
            }
            else if (type == 2)
            {
                units_1[(int)conv].Recv(bytes);
            }
        }
    }
}