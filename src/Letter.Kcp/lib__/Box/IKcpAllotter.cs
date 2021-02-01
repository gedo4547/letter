using System;

namespace Letter.Kcp.lib__
{
    interface IKcpAllotter<T> : IDisposable
    {
        T Get();

        void Put(T item);
    }
}
