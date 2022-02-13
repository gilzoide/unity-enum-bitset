using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace EnumBitSet
{
    [Serializable]
    public readonly struct EnumBitMask32<T> : IBitMask<EnumBitMask32<T>, T>
        where T : Enum
    {
        public int Mask => _data;
        
        private readonly int _data;

        public EnumBitMask32(int data)
        {
            _data = data;
        }

        #region IBitOperable<EnumBitMask32<T>, T>

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
            Contract.Requires(Convert.ToInt32(data) < 32);
            return new EnumBitMask32<T>(1 << Convert.ToInt32(data));
        }

        public int CountSetBits()
        {
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
            return BitOperations.PopCount(_data);
#else
            var count = 0;
            foreach (int bitIndex in Commons.EnumerateSetBits(_data))
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

        #region IEquatable<EnumBitMask32<T>>
        
        public bool Equals(EnumBitMask32<T> other)
        {
            return _data == other._data;
        }
        
        #endregion

        #region IEnumerable<T>
        
        public IEnumerator<T> GetEnumerator()
        {
            foreach (int bitIndex in Commons.EnumerateSetBits(_data))
            {
                yield return (T) Enum.ToObject(typeof(T), bitIndex);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
