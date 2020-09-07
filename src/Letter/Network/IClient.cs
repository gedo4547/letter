﻿using System;
using System.Net;

namespace Letter
{
    public interface IClient<TOptions> : INetwork<TOptions>, IAsyncDisposable
        where TOptions : class, IOptions
    {
        string Id { get; }

        EndPoint LocalAddress { get; }
        
        EndPoint RemoteAddress { get; }

        void AddExceptionListener(Action<Exception> onException);
    }
}