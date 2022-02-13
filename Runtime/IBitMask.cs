using System;
using System.Collections.Generic;

namespace EnumBitSet
{
    public interface IBitMask<TData, TConvertible> : IEquatable<TData>, IEnumerable<TConvertible>
        where TData : struct
        where TConvertible : IConvertible
    {
        TData BitAnd(TData other);
        TData BitOr(TData other);
        TData BitXor(TData other);
        TData BitNot();
        
        TData GetBitMask(TConvertible data);
        
        int CountSetBits();
        bool HaveSetBits();
    }
}
