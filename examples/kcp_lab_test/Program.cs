using System;
using System.Buffers;

namespace kcp_lab_test
{
    class Program
    {
        static KcpUnit unit1; 
        static KcpUnit unit2;
        static void Main(string[] args)
        {
            Console.Title = "kcp_lib";

            
            
            unit1 = new KcpUnit(SlabMemoryPoolFactory.Create(new MemoryPoolOptions(4096, 32)));
            unit1.SetRcvEvent((ref ReadOnlySequence<byte> sequence) => { OnRcvEvent(sequence, 1); });
            unit1.SetRcvEvent((ref ReadOnlySequence<byte> sequence) => { OnSndEvent(sequence, 1); });

            unit2 = new KcpUnit(SlabMemoryPoolFactory.Create(new MemoryPoolOptions(4096, 32)));
            unit2.SetRcvEvent((ref ReadOnlySequence<byte> sequence) => { OnRcvEvent(sequence, 2); });
            unit2.SetRcvEvent((ref ReadOnlySequence<byte> sequence) => { OnSndEvent(sequence, 2); });
        }
        
        private static void OnRcvEvent(in ReadOnlySequence<byte> sequence, int p1)
        {
            
        }
        
        private static void OnSndEvent(in ReadOnlySequence<byte> sequence, int p1)
        {
            
        }
    }
}