using System.Buffers;
using System.Collections.Generic;

namespace Letter
{
    public ref struct ChannelArgs
    {
        public object item;

        public List<ReadOnlySequence<byte>> buffers;
    }
}