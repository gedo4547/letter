using System.Buffers;
using System.Collections.Generic;

namespace Letter
{
    public ref struct ChannelArgs
    {
        public List<object> items;

        public List<ReadOnlySequence<byte>> buffers;
    }
}