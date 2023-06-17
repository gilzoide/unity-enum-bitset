using System;
using System.Collections.Generic;

namespace Gilzoide.EnumBitSet
{
    [Serializable]
    public class EnumBitSet64<T> : EnumBitSet<T, EnumBitMask64<T>>
        where T : struct, Enum
    {
        public EnumBitSet64() {}
        public EnumBitSet64(T value) : base(value) {}
        public EnumBitSet64(EnumBitMask64<T> value) : base(value) {}
        public EnumBitSet64(IEnumerable<T> values) : base(values) {}
        public EnumBitSet64(params T[] values) : base(values) {}
    }
}
