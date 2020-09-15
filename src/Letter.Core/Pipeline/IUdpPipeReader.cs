﻿namespace Letter
{
    public interface IUdpPipeReader
    {
        void ReceiveAsync();

        UdpMessageNode Read();
    }
}