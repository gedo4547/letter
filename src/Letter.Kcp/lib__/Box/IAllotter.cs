using System;

namespace Letter.Kcp.lib__
{
    interface IAllotter<T> : IDisposable
    {
        T Get();

        void Put(T item);
    }
}
