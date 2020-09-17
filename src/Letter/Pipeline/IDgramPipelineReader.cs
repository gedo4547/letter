﻿namespace Letter
{
    public interface IDgramPipelineReader
    {
        void ReceiveAsync();

        DgramMessageNode Read();
    }
}