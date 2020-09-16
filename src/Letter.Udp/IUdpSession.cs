﻿using System.Buffers;
using System.Net;
using System.Threading.Tasks;
using Letter;

namespace Letter.Udp
{
    public interface IUdpSession : ISession
    {
        Task WriteAsync(EndPoint remoteAddress, object obj);
        Task WriteAsync(EndPoint remoteAddress, ref ReadOnlySequence<byte> sequence);
    }
}