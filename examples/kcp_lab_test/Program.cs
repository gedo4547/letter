using System;
using System.Buffers;
using System.Text;
using System.Threading;

namespace kcp_lab_test
{
    class Program
    {
        static KcpUnit unit1; 
        static KcpUnit unit2;

        private static int num = 0;

        private static bool use_buffer = false;

        static void Main(string[] args)
        {
            Console.Title = "kcp_lib1111111111111111111111111111111";

            
            
            unit1 = new KcpUnit(SlabMemoryPoolFactory.Create(new MemoryPoolOptions(4096, 32)));
            unit1.SetRcvEvent((ref ReadOnlySequence<byte> sequence) => { OnRcvEvent(ref sequence, 1); });
            unit1.SetSndEvent((ref ReadOnlySequence<byte> sequence) => { OnSndEvent(ref sequence, 1); });

            unit2 = new KcpUnit(SlabMemoryPoolFactory.Create(new MemoryPoolOptions(4096, 32)));
            unit2.SetRcvEvent((ref ReadOnlySequence<byte> sequence) => { OnRcvEvent(ref sequence, 2); });
            unit2.SetSndEvent((ref ReadOnlySequence<byte> sequence) => { OnSndEvent(ref sequence, 2); });


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
                    return;
                }
                else if (str == "d")
                {
                    unit1.Debug();
                    //unit2.Debug();
                }

            }
        }
        
        private static void OnRcvEvent(ref ReadOnlySequence<byte> sequence, int type)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in sequence)
            {
                sb.Append(Encoding.UTF8.GetString(item.Span));
            }
            
            Console.WriteLine($"{type}>rcv>>" + sb.ToString());
        }
        
        private static void OnSndEvent(ref ReadOnlySequence<byte> sequence, int type)
        {
            //Console.WriteLine($"{type}>snd>>" + sequence.Length);
            byte[] bytes = new byte[sequence.Length];
            sequence.CopyTo(bytes);

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