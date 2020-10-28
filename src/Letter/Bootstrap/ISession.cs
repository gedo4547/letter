﻿using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO.Pipelines;
using System.Net;

namespace Letter.Bootstrap
{
    public interface ISession : IAsyncDisposable
    {
        string Id { get; }

        BinaryOrder Order { get; }

        EndPoint LoaclAddress { get; }
        
        EndPoint RemoteAddress { get; }
        
        MemoryPool<byte> MemoryPool { get; }

        PipeScheduler Scheduler { get; }
    }
}