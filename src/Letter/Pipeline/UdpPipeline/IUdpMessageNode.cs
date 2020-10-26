using System;
using System.ComponentModel;
using System.Net;

namespace Letter
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    interface IUdpMessageNode
    {
        int Length { get; }
        
        void SettingPoint(EndPoint point);
        
        void Write(ref ReadOnlyMemory<byte> memory);

        ReadOnlyMemory<byte> Read();
    }
}