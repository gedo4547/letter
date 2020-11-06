using System.Runtime.CompilerServices;

namespace Letter.IO
{
    public sealed class WrappedArgs
    {
        public object Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }
    }
}
