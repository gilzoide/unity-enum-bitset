using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace EnumBitSet
{
    [Serializable]
    public readonly struct EnumBitMask64<T> : IBitMask<EnumBitMask64<T>, T>
        where T : Enum
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

        public EnumBitMask64<T> BitAnd(EnumBitMask64<T> other)
        {
            return new EnumBitMask64<T>(_data & other._data);
        }

        public EnumBitMask64<T> BitOr(EnumBitMask64<T> other)
        {
            return new EnumBitMask64<T>(_data | other._data);
        }

        public EnumBitMask64<T> BitXor(EnumBitMask64<T> other)
        {
            return new EnumBitMask64<T>(_data ^ other._data);
        }

        public EnumBitMask64<T> BitNot()
        {
            return new EnumBitMask64<T>(~_data);
        }

        public EnumBitMask64<T> GetBitMask(T data)
        {
            return new EnumBitMask64<T>(GetLongBitMask(data));
        }
        
        public EnumBitMask64<T> GetBitMask(IEnumerable<T> data)
        {
            return new EnumBitMask64<T>(GetLongBitMask(data));
        }

        public int CountSetBits()
        {
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
            return BitOperations.PopCount(_data);
#else
            var count = 0;
            foreach (long bitIndex in Commons.EnumerateSetBits(_data))
            {
                count++;
            }
            return count;
#endif
        }

        public bool HaveSetBits()
        {
            return _data != 0;
        }
        
        #endregion

        #region IEquatable<EnumBitMask64<T>>
        
        public bool Equals(EnumBitMask64<T> other)
        {
            return _data == other._data;
        }
        
        #endregion

        #region IEnumerable<T>
        
        public IEnumerator<T> GetEnumerator()
        {
            foreach (long bitIndex in Commons.EnumerateSetBits(_data))
            {
                yield return (T) Enum.ToObject(typeof(T), bitIndex);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private static long GetLongBitMask(T data)
        {
            Contract.Requires(Convert.ToInt32(data) < 64);
            return 1L << Convert.ToInt32(data);
        }

        private static long GetLongBitMask(IEnumerable<T> data)
        {
            long mask = 0;
            foreach (T value in data)
            {
                mask |= GetLongBitMask(value);
            }
            return mask;
        }
    }
}
