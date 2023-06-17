using System;
using System.Collections.Generic;

namespace Gilzoide.EnumBitSet
{
    [Serializable]
    public class EnumBitSet32<T> : EnumBitSet<T, EnumBitMask32<T>>
        where T : struct, Enum
    {
        public EnumBitSet32() {}
        public EnumBitSet32(T value) : base(value) {}
        public EnumBitSet32(EnumBitMask32<T> value) : base(value) {}
        public EnumBitSet32(IEnumerable<T> values) : base(values) {}
        public EnumBitSet32(params T[] values) : base(values) {}
    }
}
