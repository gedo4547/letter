using System;

namespace System.Net
{
    interface IKcpAllotter<T> : IDisposable
    {
        T Get();

        void Put(T item);
    }
}
