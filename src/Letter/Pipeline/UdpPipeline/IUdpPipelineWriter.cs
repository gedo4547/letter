﻿using System;
using System.ComponentModel;
using System.Net;

namespace Letter
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IUdpPipelineWriter
    {
        UdpMessageNode GetDgramNode();
        
        void Write(EndPoint point, byte[] bytes);
        void Write(EndPoint point, byte[] bytes, int offset, int count);
        void Write(EndPoint point, ref ReadOnlyMemory<byte> memory);
        
        void Write(UdpMessageNode node);
    }
}