﻿using System.ComponentModel;
 using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class TaskExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NoAwait(this Task task)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NoAwait(this ValueTask task)
        {
        }
    }
}
