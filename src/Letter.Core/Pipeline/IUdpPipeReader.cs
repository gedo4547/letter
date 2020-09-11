﻿namespace Letter.IO
{
    public interface IUdpPipeReader
    {
        void ReceiveAsync();

        UdpMessageNode Read();
    }
}