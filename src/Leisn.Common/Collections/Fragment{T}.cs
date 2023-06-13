// By Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections;
using System.Collections.Generic;

namespace Leisn.Common.Collections
{
    public struct Fragment<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>
    {
        private int _start;
        private int _end;
        public T[] Array { get; }

        public Fragment(T[] array) : this(array, 0, array.Length)
        {
        }
        public Fragment(T[] array, int start, int end)
        {
            Array = array ?? throw new ArgumentNullException(nameof(array));
            if (start < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }

            if (end >= array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(end));
            }

            if (end - start < 0)
            {
                throw new ArgumentOutOfRangeException($"'{nameof(start)}' cannot greater than '{nameof(end)}' :{start} > {end}");
            }

            _start = start;
            _end = end;
        }

        public int Start
        {
            get => _start;
            set
            {
                if (value < 0 || value > End)
                {
                    throw new ArgumentOutOfRangeException(nameof(Start));
                }

                _start = value;
            }
        }
        public int End
        {
            get => _end;
            set
            {
                if (value >= Array.Length || value < Start)
                {
                    throw new ArgumentOutOfRangeException(nameof(End));
                }

                _end = value;
            }
        }

        public bool IsEmpty => Count < 1;
        public int Count => End - Start;
        public bool IsReadOnly { get; } = true;

        public T this[int index] => index < Start || index > End ? throw new ArgumentOutOfRangeException(nameof(index)) : Array[index + Start];

        T IList<T>.this[int index]
        {
            get => this[index];
            set => throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            int index = -1;
            for (int i = Start; i <= End; i++)
            {
                index++;
                if (Equals(item, Array[i]))
                {
                    return index;
                }
            }
            return index;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            System.Array.Copy(Array, Start, array, arrayIndex, Count);
        }

        #region enumerator
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private struct Enumerator : IEnumerator<T>, IEnumerator
        {
            public T Current => _fragment.Array[_current];

            private int _current;

            private Fragment<T> _fragment;

            public Enumerator(Fragment<T> fragment)
            {
                _fragment = fragment;
                _current = fragment.Start - 1;
            }

            public bool MoveNext()
            {
                _current++;
                return _current < _fragment.End;
            }

            object IEnumerator.Current => Current!;

            public void Reset()
            {
                _current = _fragment.Start - 1;
            }

            void IDisposable.Dispose()
            {
            }
        }
        #endregion

        #region not supported

        void IList<T>.Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
