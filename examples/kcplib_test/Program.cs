using System;
using System.Buffers;
using Letter.Kcp.lib__;

namespace kcplib_test
{
    class Program
    {
        static void Main(string[] args)
        {
            Kcp kcp = new Kcp(1, true, SlabMemoryPoolFactory.shared, (bytes, i) =>
            {
                
            });
            
            
        }
    }
}