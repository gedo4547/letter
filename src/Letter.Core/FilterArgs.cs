using System.Buffers;

namespace Letter
{
    public ref struct FilterArgs
    {
        public object item;
        public ReadOnlySequence<byte> buffer;
    }
}