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
        private readonly uint _data;

        public EnumBitMask32(uint data)
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
            return new EnumBitMask32<T>(1u << Convert.ToInt32(data));
        }

        public int CountSetBits()
        {
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
            return BitOperations.PopCount(_data);
#else
            var count = 0;
            var data = _data;
            while (data != 0)
            {
                if ((data & 1) != 0)
                {
                    count++;
                }
                data >>= 1;
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
        
        public IEnumerator<T> GetEnumerator()
        {
            var i = 0;
            var data = _data;
            while (data != 0)
            {
                if ((data & 1) != 0)
                {
                    yield return (T) Enum.ToObject(typeof(T), i);
                }

                i++;
                data >>= 1;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
