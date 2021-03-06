﻿namespace Letter.IO
{
    public interface IFilterPipeline<TSession> where TSession : ISession
    {
        void Add(IFilter<TSession> filter);
    }
}