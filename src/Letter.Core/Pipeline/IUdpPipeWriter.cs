﻿using System;
using System.Net;

namespace Letter.IO
{
    public interface IUdpPipeWriter
    {
        UdpMessageNode GetDgramNode();
        
        void Write(EndPoint point, byte[] bytes);
        void Write(EndPoint point, byte[] bytes, int offset, int count);
        void Write(EndPoint point, ref ReadOnlyMemory<byte> memory);
        
        void Write(UdpMessageNode node);
    }
}