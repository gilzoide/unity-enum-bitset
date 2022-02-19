using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace EnumBitSet
{
    [Serializable]
    public readonly struct EnumBitMask32<T> : IBitMask<EnumBitMask32<T>, T>
        where T : struct, Enum
    {
        public int Mask => _data;
        
        private readonly int _data;

        public EnumBitMask32(int data)
        {
            _data = data;
        }

        public EnumBitMask32(T value)
        {
            _data = GetIntBitMask(value);
        }

        public EnumBitMask32(IEnumerable<T> values)
        {
            _data = GetIntBitMask(values);
        }

        public EnumBitMask32(params T[] values) : this((IEnumerable<T>) values) {}

        #region IBitMask<EnumBitMask32<T>, T>

        public EnumBitMask32<T> BitAnd(EnumBitMask32<T> other)
        {
            return new EnumBitMask32<T>(_data & other._data);
        }

        public EnumBitMask32<T> BitOr(EnumBitMask32<T> other)
        {
            return new EnumBitMask32<T>(_data | other._data);
        }

        public EnumBitMask32<T> BitXor(EnumBitMask32<T> other)
        {
            return new EnumBitMask32<T>(_data ^ other._data);
        }

        public EnumBitMask32<T> BitNot()
        {
            return new EnumBitMask32<T>(~_data);
        }

        public EnumBitMask32<T> GetBitMask(T data)
        {
            return new EnumBitMask32<T>(GetIntBitMask(data));
        }
        
        public EnumBitMask32<T> GetBitMask(IEnumerable<T> data)
        {
            return new EnumBitMask32<T>(GetIntBitMask(data));
        }

        public bool HaveSetBits()
        {
            return _data != 0;
        }
        
        #endregion

        #region IReadOnlySet<T>

        public bool Contains(T item)
        {
            var mask = GetIntBitMask(item);
            return (_data & mask) == mask;
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            var mask = GetIntBitMask(other);
            return (_data & mask) == _data && (mask & ~_data) != 0;
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            var mask = GetIntBitMask(other);
            return (mask & _data) == mask && (_data & ~mask) != 0;
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            var mask = GetIntBitMask(other);
            return (_data & mask) == _data;
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            var mask = GetIntBitMask(other);
            return (mask & _data) == mask;
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            var mask = GetIntBitMask(other);
            return (_data & mask) != 0;
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            var mask = GetIntBitMask(other);
            return _data == mask;
        }

        #endregion

        #region IReadOnlyCollection<T>

        public int Count => Commons.CountSetBits(_data);

        #endregion

        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var bitIndex in Commons.EnumerateSetBits(_data))
            {
                var value = EnumMetadata<T>.HasFlags ? 1 << bitIndex : bitIndex;
                yield return Commons.IntToEnum<T>(value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEquatable<EnumBitMask32<T>>
        
        public bool Equals(EnumBitMask32<T> other)
        {
            return _data == other._data;
        }
        
        #endregion

        private static int GetIntBitMask(T data)
        {
            if (EnumMetadata<T>.HasFlags)
            {
                return Commons.EnumToInt(data);
            }
            Contract.Requires(Commons.EnumToInt(data) < 32);
            return 1 << Commons.EnumToInt(data);
        }

        private static int GetIntBitMask(IEnumerable<T> other)
        {
            switch (other)
            {
                case EnumBitMask32<T> bitmask:
                    return bitmask._data;
                
                case null:
                    throw new ArgumentNullException(nameof(other), "Value cannot be null.");

                default:
                    var mask = 0;
                    foreach (T value in other)
                    {
                        mask |= GetIntBitMask(value);
                    }
                    return mask;
            }
        }
    }
}
