﻿using Letter;

namespace Letter.Udp
{
    public interface IUdpBootstrap : IDgramBootstrap<UdpOptions, IUdpSession, IUdpChannel, IUdpNetwork>
    {
        
    }
}