using System.Buffers;

namespace Letter
{
    public ref struct TransportArgs
    {
        public object item;
        public ReadOnlySequence<byte> sequence;
    }
}