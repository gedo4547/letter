using System.Buffers;

namespace Letter.Box
{
    public ref struct MessageArgs
    {
        public object item;
        public ReadOnlySequence<byte> sequence;
    }
}