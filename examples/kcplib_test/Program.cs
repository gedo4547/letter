using System;
using System.Buffers;
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
            unit1 = new KcpUnit();
            unit1.onRcvEvent = (memory)=>{ OnRcvEvent(memory, 1); };
            unit1.onSndEvent = (memory)=>{ OnSndEvent(memory, 1); };

            unit2 = new KcpUnit();
            unit2.onRcvEvent = (memory)=>{ OnRcvEvent(memory, 2); };
            unit2.onSndEvent = (memory)=>{ OnSndEvent(memory, 2); };
            
            while(true)
            {
                // System.Threading.Thread.Sleep(1);
                Console.ReadLine();
                for (int i = 0; i < 1; i++)
                {
                    Interlocked.Increment(ref num);
                    unit1.Send(System.Text.Encoding.UTF8.GetBytes($"nihao {num}"));
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