using System;
using System.Buffers;
using System.Buffers.Binary;
using Letter.IO;

namespace Letter.Kcp
{
    public class KcpOptions : IOptions
    {
        public int? RcvBufferSize { get; set; }
        public int? SndBufferSize { get; set; }
        public int? RcvTimeout { get; set; }
        public int? SndTimeout { get; set; }
        public BinaryOrder Order { get; set; } = BinaryOrder.BigEndian;
        public MemoryPoolOptions MemoryPoolOptions { get; set; } = new MemoryPoolOptions(4096, 32);
        public int SchedulerCount { get; set; } = Math.Min(Environment.ProcessorCount, 16);
        
        public NoDelayConfig NoDelay { get; set; } = new NoDelayConfig(1, 10, 2, 1);
        public WndSizeConfig WndSize { get; set; } = new WndSizeConfig(256, 256);
        public int Mtu { get; set; } = 512;

        public int interval = 10;
    }
    
    public class NoDelayConfig
    {
        public int nodelay_;
        public int interval_;
        public int resend_;
        public int nc_;

        public NoDelayConfig(int nodelay_, int interval_, int resend_, int nc_)
        {
            this.nodelay_ = nodelay_;
            this.interval_ = interval_;
            this.resend_ = resend_;
            this.nc_ = nc_;
        }
    }

    public class WndSizeConfig
    {
        public int sndwnd;
        public int rcvwnd;

        public WndSizeConfig(int sndwnd, int rcvwnd)
        {
            this.sndwnd = sndwnd;
            this.rcvwnd = rcvwnd;
        }
    }
}