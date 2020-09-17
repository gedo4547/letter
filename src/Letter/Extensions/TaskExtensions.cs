﻿using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
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
