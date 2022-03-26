using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Gilzoide.EnumBitSet
{
    [Serializable]
    public readonly struct EnumBitMask64<T> : IBitMask<EnumBitMask64<T>, T>, IEquatable<EnumBitMask64<T>>
        where T : struct, Enum
    {
        public long Mask => _data;
        
        private readonly long _data;

        public EnumBitMask64(long data)
        {
            _data = data;
        }

        public EnumBitMask64(T value)
        {
            _data = GetLongBitMask(value);
        }

        public EnumBitMask64(IEnumerable<T> values)
        {
            _data = GetLongBitMask(values);
        }

        public EnumBitMask64(params T[] values) : this((IEnumerable<T>) values) {}

        #region IBitMask<EnumBitMask64<T>, T>

        public EnumBitMask64<T> Union(T value)
        {
            var mask = GetLongBitMask(value);
            return new EnumBitMask64<T>(_data | mask);
        }

        public EnumBitMask64<T> Union(IEnumerable<T> other)
        {
            var mask = GetLongBitMask(other);
            return new EnumBitMask64<T>(_data | mask);
        }

        public EnumBitMask64<T> Intersection(IEnumerable<T> other)
        {
            var mask = GetLongBitMask(other);
            return new EnumBitMask64<T>(_data & mask);
        }

        public EnumBitMask64<T> Difference(T value)
        {
            var mask = GetLongBitMask(value);
            return new EnumBitMask64<T>(_data & ~mask);
        }

        public EnumBitMask64<T> Difference(IEnumerable<T> other)
        {
            var mask = GetLongBitMask(other);
            return new EnumBitMask64<T>(_data & ~mask);
        }

        public EnumBitMask64<T> SymmetricDifference(IEnumerable<T> other)
        {
            var mask = GetLongBitMask(other);
            return new EnumBitMask64<T>(_data ^ mask);
        }

        public EnumBitMask64<T> GetBitMask(T data)
        {
            return new EnumBitMask64<T>(GetLongBitMask(data));
        }
        
        public EnumBitMask64<T> GetBitMask(IEnumerable<T> data)
        {
            return new EnumBitMask64<T>(GetLongBitMask(data));
        }

        public bool HaveSetBits()
        {
            return _data != 0;
        }
        
        #endregion

        #region IReadOnlySet<T>

        public bool Contains(T item)
        {
            var mask = GetLongBitMask(item);
            return (_data & mask) == mask;
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            var mask = GetLongBitMask(other);
            return (_data & mask) == _data && (mask & ~_data) != 0;
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            var mask = GetLongBitMask(other);
            return (mask & _data) == mask && (_data & ~mask) != 0;
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            var mask = GetLongBitMask(other);
            return (_data & mask) == _data;
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            var mask = GetLongBitMask(other);
            return (mask & _data) == mask;
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            var mask = GetLongBitMask(other);
            return (_data & mask) != 0;
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            var mask = GetLongBitMask(other);
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
                var value = EnumMetadata<T>.HasFlags ? 1L << bitIndex : bitIndex;
                yield return Commons.LongToEnum<T>(value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEquatable<EnumBitMask64<T>>
        
        public bool Equals(EnumBitMask64<T> other)
        {
            return _data == other._data;
        }
        
        #endregion

        private static long GetLongBitMask(T data)
        {
            if (EnumMetadata<T>.HasFlags)
            {
                return Commons.EnumToLong(data);
            }
            Contract.Requires(Commons.EnumToInt(data) < 64);
            return 1L << Commons.EnumToInt(data);
        }

        private static long GetLongBitMask(IEnumerable<T> other)
        {
            switch (other)
            {
                case EnumBitMask64<T> bitmask:
                    return bitmask._data;
                
                case null:
                    throw new ArgumentNullException(nameof(other), "Value cannot be null.");

                default:
                    var mask = 0L;
                    foreach (T value in other)
                    {
                        mask |= GetLongBitMask(value);
                    }
                    return mask;
            }
        }
    }
}
