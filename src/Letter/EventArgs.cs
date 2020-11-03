using System;
using System.Runtime.CompilerServices;

namespace Letter
{
    public sealed class EventArgs
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
