using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Net.Sockets;
using Letter.IO;

namespace Letter.Tcp
{
    public class ATcpOptions : IOptions
    {
        /// <summary>
        /// Set to false to enable Nagle's algorithm for all connections.
        /// </summary>
        /// <remarks>
        /// Defaults to true.
        /// </remarks>
        public bool NoDelay { get; set; } = true;
        public bool KeepAlive { get; set; } = true;
        public int? RcvBufferSize { get; set; }
        public int? SndBufferSize { get; set; }
        public int? RcvTimeout { get; set; }
        public int? SndTimeout { get; set; }
        
        public BinaryOrder Order { get; set; } = BinaryOrder.BigEndian;
        public LingerOption LingerOption { get; set; } = new LingerOption(false, 0);
        
        public long? MaxPipelineReadBufferSize { get; set; } = 1024 * 1024;
        public long? MaxPipelineWriteBufferSize { get; set; } = 64 * 1024;

        public MemoryPoolOptions MemoryPoolOptions { get; set; } = new MemoryPoolOptions(4096, 32);

        public int SchedulerCount { get; set; } = Math.Min(Environment.ProcessorCount, 16);
    }
}