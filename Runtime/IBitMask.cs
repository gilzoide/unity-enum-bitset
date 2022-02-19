using System;
using System.Collections.Generic;

namespace EnumBitSet
{
    public interface IBitMask<TData, TConvertible> : IReadOnlySet<TConvertible>, IEquatable<TData>
        where TData : struct
        where TConvertible : IConvertible
    {
        TData BitAnd(TData other);
        TData BitOr(TData other);
        TData BitXor(TData other);
        TData BitNot();
        
        TData GetBitMask(TConvertible data);
        TData GetBitMask(IEnumerable<TConvertible> data);
        
        bool HaveSetBits();
    }
    
#if !NET5_0_OR_GREATER
        public interface IReadOnlySet<T> : IReadOnlyCollection<T>
        {
            bool Contains(T value);
            bool IsProperSubsetOf(IEnumerable<T> other);
            bool IsProperSupersetOf(IEnumerable<T> other);
            bool IsSubsetOf(IEnumerable<T> other);
            bool IsSupersetOf(IEnumerable<T> other);
            bool Overlaps(IEnumerable<T> other);
            bool SetEquals(IEnumerable<T> other);
        }
#endif
}
