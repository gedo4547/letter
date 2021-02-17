using System;
using System.Buffers;
using System.Diagnostics;
using System.Threading;
using Letter.Kcp.lib__;

namespace kcplib_test
{
    class Program
    {

        static KcpUnit unit1; 
        static KcpUnit unit2;

        private static int num = 0;

        static void Main(string[] args)
        {
            //progress
            Console.WriteLine(Process.GetCurrentProcess().Id); 
            
            
            Console.Title = "kcp_lib";
            
            unit1 = new KcpUnit();
            unit1.onRcvEvent = (memory)=>{ OnRcvEvent(memory, 1); };
            unit1.onSndEvent = (memory)=>{ OnSndEvent(memory, 1); };

            unit2 = new KcpUnit();
            unit2.onRcvEvent = (memory)=>{ OnRcvEvent(memory, 2); };
            unit2.onSndEvent = (memory)=>{ OnSndEvent(memory, 2); };
            
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(common.Message.data_1024);
            while(true)
            {
                // System.Threading.Thread.Sleep(1);
                string str = Console.ReadLine(); 

                if(int.TryParse(str, out var length))
                {
                    for (int i = 0; i < length; i++)
                    {
                        Interlocked.Increment(ref num);
                        unit1.Send(System.Text.Encoding.UTF8.GetBytes($"nihao {i}"));
                        // unit1.Send(buffer);
                    }
                }
                else if(string.IsNullOrEmpty(str))
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        Interlocked.Increment(ref num);
                        unit1.Send(System.Text.Encoding.UTF8.GetBytes($"nihao {i}"));
                        // unit1.Send(buffer);
                    }
                }
                else if(str == "s")
                {
                    return;
                }
                else if(str == "d")
                {
                    unit1.Debug();
                    unit2.Debug();
                }
                
            }
        }

        private static void OnRcvEvent(Memory<byte> memory, int v)
        {
            var str = System.Text.Encoding.UTF8.GetString(memory.Span);
            Console.WriteLine($"{v}>rcv>>"+str);
        }

        private static void OnSndEvent(Memory<byte> memory, int v)
        {
            Console.WriteLine($"{v}>snd>>" + memory.Length);
            if(v == 1)
            {
                unit2.Recv(memory);
            }
            else if(v == 2)
            {
                unit1.Recv(memory);
            }
        }
    }
}