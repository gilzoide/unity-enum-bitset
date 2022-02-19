using System;
using System.Collections.Generic;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace EnumBitSet
{
    [Serializable]
    public class EnumBitSet64<T> : EnumBitSet<T, EnumBitMask64<T>>
#if UNITY_5_3_OR_NEWER
        , ISerializationCallbackReceiver
#endif
        where T : struct, Enum
    {
        public EnumBitSet64() {}
        public EnumBitSet64(T value) : base(value) {}
        public EnumBitSet64(IEnumerable<T> values) : base(values) {}
        public EnumBitSet64(params T[] values) : base(values) {}

#if UNITY_5_3_OR_NEWER
        [SerializeField] private long _bitMask;

        public void OnAfterDeserialize()
        {
            _data = new EnumBitMask64<T>(_bitMask);
        }

        public void OnBeforeSerialize()
        {
            _bitMask = _data.Mask;
        }
#endif
    }
}
