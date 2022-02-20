using System.Collections.Generic;

namespace EnumBitSet
{
    public interface IBitMask<TData, T> : IReadOnlySet<T>
    {
        TData Union(T value);
        TData Union(IEnumerable<T> other);
        TData Intersection(IEnumerable<T> other);
        TData Difference(T value);
        TData Difference(IEnumerable<T> other);
        TData SymmetricDifference(IEnumerable<T> other);
        
        TData GetBitMask(T data);
        TData GetBitMask(IEnumerable<T> data);
        
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
