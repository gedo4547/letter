﻿namespace Letter
{
    public interface IDgramPipeReader
    {
        void ReceiveAsync();

        DgramMessageNode Read();
    }
}