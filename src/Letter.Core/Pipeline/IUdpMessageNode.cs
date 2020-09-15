﻿using System;
using System.Net;

namespace Letter
{
    interface IUdpMessageNode
    {
        int Length { get; }
        
        void SettingPoint(EndPoint point);
        
        void Write(ref ReadOnlyMemory<byte> memory);

        ReadOnlyMemory<byte> Read();
    }
}