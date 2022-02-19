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

        #region IEquatable<EnumBitMask32<T>>
        
        public bool Equals(EnumBitMask32<T> other)
        {
            return _data == other._data;
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
                yield return Commons.IntToEnum<T>(bitIndex);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private static int GetIntBitMask(T data)
        {
            Contract.Requires(Commons.EnumToInt(data) < 32);
            return 1 << Commons.EnumToInt(data);
        }

        private static int GetIntBitMask(IEnumerable<T> data)
        {
            int mask = 0;
            foreach (T value in data)
            {
                mask |= GetIntBitMask(value);
            }
            return mask;
        }
    }
}
