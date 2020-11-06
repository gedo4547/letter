using System.Runtime.CompilerServices;

namespace System.IO.Pipelines
{
    public class BufferStack<T>
    {
        private BufferAsValueType[] _array;
        private int _size;

        public BufferStack(int size)
        {
            _array = new BufferAsValueType[size];
            _size = 0;
        }

        public int Count => _size;

        public bool TryPop(out T result)
        {
            int size = _size - 1;
            BufferAsValueType[] array = _array;

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
            BufferAsValueType[] array = _array;

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
        
        private readonly struct BufferAsValueType
        {
            private readonly T _value;
            private BufferAsValueType(T value) => _value = value;
            public static implicit operator BufferAsValueType(T s) => new BufferAsValueType(s);
            public static implicit operator T(BufferAsValueType s) => s._value;
        }
    }
}