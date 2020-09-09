using System;
using System.Buffers;

namespace Letter
{
    public ref struct EventArgs
    {
        public object item;
        public ReadOnlySequence<byte> buffer;
    }
}