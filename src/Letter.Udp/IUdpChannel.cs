﻿using Letter.IO;

namespace Letter.Udp
{
    public interface IUdpChannel : IChannel<IUdpContext, WrappedDgramReader, WrappedDgramWriter>
    {
        
    }
}