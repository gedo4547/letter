using System;

namespace System.Net
{
    interface IAllotter<T> : IDisposable
    {
        T Get();

        void Put(T item);
    }
}
