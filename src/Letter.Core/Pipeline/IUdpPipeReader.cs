﻿namespace Letter.IO
{
    interface IUdpPipeReader
    {
        void ReceiveAsync();

        UdpMessageNode Read();
    }
}