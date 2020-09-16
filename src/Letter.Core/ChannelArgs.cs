using System.Buffers;

namespace Letter
{
    public ref struct ChannelArgs
    {
        public object item;
        public ReadOnlySequence<byte> buffer;
    }
}