using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Gilzoide.EnumBitSet
{
    [Serializable]
    public class EnumBitSet<T, TData> : ISet<T>
        , IReadOnlySet<T>
#if UNITY_5_3_OR_NEWER
        , ISerializationCallbackReceiver
#endif
        where T : struct, Enum
        where TData : struct, IBitMask<TData, T>
    {
        protected TData _data;

        public EnumBitSet() {}

        public EnumBitSet(TData value)
        {
            _data = value;
        }

        public EnumBitSet(T value)
        {
            _data = _data.GetBitMask(value);
        }

        public EnumBitSet(IEnumerable<T> values)
        {
            _data = _data.GetBitMask(values);
        }

        public EnumBitSet(params T[] values) : this((IEnumerable<T>) values) {}

        public bool Any()
        {
            return _data.HaveSetBits();
        }

        public static implicit operator TData(EnumBitSet<T, TData> self)
        {
            return self._data;
        }

        public static implicit operator EnumBitSet<T, TData>(TData data)
        {
            return new EnumBitSet<T, TData>(data);
        }
        
        #region ISet<T>

        public void ExceptWith(IEnumerable<T> other)
        {
            _data = _data.Difference(other);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            _data = _data.Intersection(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return _data.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return _data.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return _data.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return _data.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return _data.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return _data.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            _data = _data.SymmetricDifference(other);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            _data = _data.Union(other);
        }
        
        #endregion

        #region ICollection<T>
        
        void ICollection<T>.Add(T item)
        {
            _data = _data.Union(item);
        }

        public bool Add(T item)
        {
            if (_data.Contains(item))
            {
                return false;
            }
            _data = _data.Union(item);
            return true;
        }

        public void Clear()
        {
            _data = new TData();
        }

        public bool Contains(T item)
        {
            return _data.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array), "Value cannot be null.");
            }
            else if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Non negative number is required.");
            }
            else if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
            }
            using (IEnumerator<T> enumerator = GetEnumerator())
            {
                for (var i = arrayIndex; enumerator.MoveNext(); i++)
                {
                    array[i] = enumerator.Current;
                }
            }
        }

        public bool Remove(T item)
        {
            if (!_data.Contains(item))
            {
                return false;
            }
            _data = _data.Difference(item);
            return true;
        }

        public int Count => _data.Count;

        public bool IsReadOnly => false;

        #endregion
        
        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

#if UNITY_5_3_OR_NEWER
        #region ISerializationCallbackReceiver

        [SerializeField] private NamedEnum[] _serializedEnums;

        public void OnAfterDeserialize()
        {
            Clear();
            if (_serializedEnums == null)
            {
                return;
            }
            foreach (NamedEnum serializedValue in _serializedEnums)
            {
                Add(serializedValue.Get<T>());
            }
        }

        public void OnBeforeSerialize()
        {
            if (_serializedEnums?.Length != Count)
            {
                _serializedEnums = new NamedEnum[Count];
            }
            
            var i = 0;
            foreach (T value in _data)
            {
                _serializedEnums[i] = new NamedEnum
                {
                    Value = Commons.EnumToInt(value),
                    Name = value.ToString(),
                };
                i++;
            }
        }

        #endregion
#endif
    }
    
#if UNITY_5_3_OR_NEWER
    [Serializable]
    internal struct NamedEnum
    {
        public int Value;
        public string Name;

        public T Get<T>() where T : struct, Enum
        {
            return Enum.TryParse(Name, out T value) ? value : Commons.IntToEnum<T>(Value);
        }
    }
#endif
}
