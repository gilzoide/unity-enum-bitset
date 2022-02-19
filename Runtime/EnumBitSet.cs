using System;
using System.Collections;
using System.Collections.Generic;

namespace EnumBitSet
{
    [Serializable]
    public class EnumBitSet<T, TData> : ISet<T>
#if NET5_0_OR_GREATER
        , IReadOnlySet<T>
#endif
        , IReadOnlyCollection<T>
        , IEquatable<EnumBitSet<T, TData>>
        where T : Enum
        where TData : struct, IBitMask<TData, T>
    {
        protected TData _data;

        public EnumBitSet() {}

        public EnumBitSet(T value)
        {
            _data = GetBitMask(value);
        }

        public EnumBitSet(IEnumerable<T> values)
        {
            _data = GetBitMask(values);
        }

        public EnumBitSet(params T[] values) : this((IEnumerable<T>) values) {}
        
        private bool this[TData mask]
        {
            get => mask.HaveSetBits() && _data.BitAnd(mask).Equals(mask);
            set => _data = value ? _data.BitOr(mask) : _data.BitAnd(mask.BitNot());
        }
        
        public bool this[T index]
        {
            get => this[GetBitMask(index)];
            set => this[GetBitMask(index)] = value;
        }

        public bool this[IEnumerable<T> index]
        {
            get => this[GetBitMask(index)];
            set => this[GetBitMask(index)] = value;
        }
        
        public bool this[params T[] indices]
        {
            get => this[(IEnumerable<T>) indices];
            set => this[(IEnumerable<T>) indices] = value;
        }

        public bool Any()
        {
            return _data.HaveSetBits();
        }
        
        #region ISet<T>

        public void ExceptWith(IEnumerable<T> other)
        {
            this[other] = false;
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            var mask = GetBitMask(other);
            _data = _data.BitAnd(mask);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            var mask = GetBitMask(other);
            return _data.BitAnd(mask).Equals(_data) && mask.BitAnd(_data.BitNot()).HaveSetBits();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            var mask = GetBitMask(other);
            return mask.BitAnd(_data).Equals(mask) && _data.BitAnd(mask.BitNot()).HaveSetBits();
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            var mask = GetBitMask(other);
            return _data.BitAnd(mask).Equals(_data);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            var mask = GetBitMask(other);
            return mask.BitAnd(_data).Equals(mask);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            var mask = GetBitMask(other);
            return _data.BitAnd(mask).HaveSetBits();
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            var mask = GetBitMask(other);
            return _data.Equals(mask);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            var mask = GetBitMask(other);
            _data = _data.BitXor(mask);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            this[other] = true;
        }
        
        #endregion

        #region ICollection<T>
        
        void ICollection<T>.Add(T item)
        {
            this[item] = true;
        }

        public bool Add(T item)
        {
            if (this[item])
            {
                return false;
            }
            this[item] = true;
            return true;
        }

        public void Clear()
        {
            _data = new TData();
        }

        public bool Contains(T item)
        {
            return this[item];
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array), "Value cannot be null.");
            }
            else if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Non negative number is required.");
            }
            else if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
            }
            using (IEnumerator<T> enumerator = GetEnumerator())
            {
                for (var i = arrayIndex; enumerator.MoveNext(); i++)
                {
                    array[i] = enumerator.Current;
                }
            }
        }

        public bool Remove(T item)
        {
            if (!this[item])
            {
                return false;
            }
            this[item] = false;
            return true;
        }

        public int Count => _data.CountSetBits();

        public bool IsReadOnly => false;

        #endregion
        
        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEquatable<EnumBitSet<T, TData>>
        
        public bool Equals(EnumBitSet<T, TData> other)
        {
            return other != null && _data.Equals(other._data);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is EnumBitSet<T, TData> other && Equals(other);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return _data.GetHashCode();
        }
        
        #endregion

        private TData GetBitMask(T value)
        {
            return _data.GetBitMask(value);
        }
        
        private TData GetBitMask(IEnumerable<T> other)
        {
            switch (other)
            {
                case TData data:
                    return data;

                case EnumBitSet<T, TData> bitset:
                    return bitset._data;                
                
                case null:
                    throw new ArgumentNullException(nameof(other), "Value cannot be null.");
                
                default:
                    return _data.GetBitMask(other);
            }
        }
    }
}
