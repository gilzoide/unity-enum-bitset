using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_2019_4_OR_NEWER
using UnityEngine;
#endif

namespace EnumBitSet
{
    [Serializable]
    public class EnumBitSet32<T> : EnumBitSet<T, EnumBitMask32<T>>
#if UNITY_2019_4_OR_NEWER
        , ISerializationCallbackReceiver
#endif
        where T : Enum
    {
        public EnumBitSet32() {}
        public EnumBitSet32(T value) : base(value) {}
        public EnumBitSet32(IEnumerable<T> values) : base(values) {}
        public EnumBitSet32(params T[] values) : base(values) {}

#if UNITY_2019_4_OR_NEWER
        [SerializeField] private uint _bitMask;

        public void OnAfterDeserialize()
        {
            _data = new EnumBitMask32<T>(_bitMask);
        }

        public void OnBeforeSerialize()
        {
            _bitMask = _data.Mask;
        }
#endif
    }
}
