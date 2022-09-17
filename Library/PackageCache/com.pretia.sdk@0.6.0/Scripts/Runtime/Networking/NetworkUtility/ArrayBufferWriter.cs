using System;
using System.Buffers;

namespace PretiaArCloud.Networking
{
    internal class ArrayBufferWriter<T> : IBufferWriter<T>, IDisposable
    {
        private T[] _buffer;
        private int _index;

        private const int DefaultInitialBufferSize = 256;

        public T[] Buffer => _buffer;
        public int Index => _index;
        public int FreeCapacity => _buffer.Length - _index;
        public ReadOnlySpan<T> WrittenSpan => _buffer.AsSpan().Slice(0, _index);

        private ArrayPool<T> _arrayPool;

        public ArrayBufferWriter()
        {
            _buffer = Array.Empty<T>();
            _index = 0;
        }

        public ArrayBufferWriter(int initialCapacity)
        {
            _buffer = new T[initialCapacity];
            _index = 0;
        }

        public ArrayBufferWriter(int initialCapacity, ArrayPool<T> arrayPool)
        {
            _arrayPool = arrayPool;
            _buffer = _arrayPool.Rent(initialCapacity);
            _index = 0;
        }

        public void Advance(int count)
        {
            _index += count;
        }

        private void CheckAndResizeBuffer(int sizeHint)
        {
            if (sizeHint > FreeCapacity)
            {
                int currentLength = _buffer.Length;
                int growBy = Math.Max(sizeHint, currentLength);

                if (currentLength == 0)
                {
                    growBy = Math.Max(growBy, DefaultInitialBufferSize);
                }

                int newSize = currentLength + growBy;

                if ((uint)newSize > int.MaxValue)
                {
                    newSize = currentLength + sizeHint;
                    if ((uint)newSize > int.MaxValue)
                    {
                        throw new System.OutOfMemoryException(string.Format("Buffer Maximum Size Exceeded {0}", newSize));
                    }
                }

                if (_arrayPool == null)
                {
                    Array.Resize(ref _buffer, newSize);
                }
                else
                {
                    var newBuffer = _arrayPool.Rent(newSize);
                    Array.Copy(_buffer, newBuffer, _index);
                    _arrayPool.Return(_buffer);

                    _buffer = newBuffer;
                }
            }
        }

        public Memory<T> GetMemory(int sizeHint = 0)
        {
            CheckAndResizeBuffer(sizeHint);
            return _buffer.AsMemory(_index);
        }

        public Span<T> GetSpan(int sizeHint = 0)
        {
            CheckAndResizeBuffer(sizeHint);
            return _buffer.AsSpan(_index);
        }

        internal void Clear()
        {
            _buffer.AsSpan(0, _index).Clear();
            _index = 0;
        }

        public void Dispose()
        {
            Clear();
            if (_arrayPool != null)
                _arrayPool.Return(_buffer);
        }
    }
}