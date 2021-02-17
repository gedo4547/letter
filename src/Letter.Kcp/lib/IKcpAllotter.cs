using System;

namespace System.Net.Sockets
{
    interface IKcpAllotter<T> : IDisposable
    {
        T Get();

        void Put(T item);
    }
}
