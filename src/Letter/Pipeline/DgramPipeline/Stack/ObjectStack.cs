using System;
using System.Runtime.CompilerServices;

namespace System.IO.Pipelines
{
    class ObjectStack<T>
    {
        private ObjectAsValueType[] _array;
        private int _size;

        public ObjectStack(int size)
        {
            _array = new ObjectAsValueType[size];
            _size = 0;
        }

        public int Count => _size;

        public bool TryPop(out T result)
        {
            int size = _size - 1;
            ObjectAsValueType[] array = _array;

            if ((uint)size >= (uint)array.Length)
            {
                result = default;
                return false;
            }

            _size = size;
            result = array[size];
            array[size] = default;
            return true;
        }

        // Pushes an item to the top of the stack.
        public void Push(T item)
        {
            int size = _size;
            ObjectAsValueType[] array = _array;

            if ((uint)size < (uint)array.Length)
            {
                array[size] = item;
                _size = size + 1;
            }
            else
            {
                PushWithResize(item);
            }
        }

        // Non-inline from Stack.Push to improve its code quality as uncommon path
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void PushWithResize(T item)
        {
            Array.Resize(ref _array, 2 * _array.Length);
            _array[_size] = item;
            _size++;
        }
        
        private readonly struct ObjectAsValueType
        {
            private readonly T _value;
            private ObjectAsValueType(T value) => _value = value;
            public static implicit operator ObjectAsValueType(T s) => new ObjectAsValueType(s);
            public static implicit operator T(ObjectAsValueType s) => s._value;
        }
    }
}