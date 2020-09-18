﻿using System.ComponentModel;
 using System.Runtime.CompilerServices;

namespace System
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SequencePositionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetPositionIndex(in this SequencePosition position)
        {
            return position.GetInteger();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object GetPositionObject(in this SequencePosition position)
        {
            return position.GetObject();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SequencePosition LeftOffet(in this SequencePosition position, int offset)
        {
            return position.Offet(offset * -1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SequencePosition RightOffset(in this SequencePosition position, int offset)
        {
            return position.Offet(offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SequencePosition Offet(in this SequencePosition position, int offset)
        {
            return new SequencePosition(position.GetObject(), position.GetInteger() + offset);
        }
    }
}
