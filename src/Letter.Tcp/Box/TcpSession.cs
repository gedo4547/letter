﻿using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Letter.Bootstrap;

namespace Letter.Tcp.Box
{
    class TcpSession : ATcpSession, ITcpSession
    {
        public TcpSession(Socket socket, ATcpOptions options, PipeScheduler scheduler, MemoryPool<byte> pool, ChannelFilterGroup<Box.ITcpSession, Box.ITcpChannelFilter> filterGroup) 
            : base(socket, options, scheduler, pool, filterGroup)
        {
        }

        public string Id { get; }
        public BinaryOrder Order { get; }
        public EndPoint LoaclAddress { get; }
        public EndPoint RemoteAddress { get; }
        public MemoryPool<byte> MemoryPool { get; }
        public PipeScheduler Scheduler { get; }

        public override Task StartAsync()
        {
            throw new System.NotImplementedException();
        }
        
        public Task WriteAsync(object o)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}