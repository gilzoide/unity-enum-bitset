using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EnumBitSet
{
    [CustomPropertyDrawer(typeof(EnumBitSet32<>))]
    public class EnumBitSetPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            UnityEngine.Object targetObject = property.serializedObject.targetObject;
            object bitset = fieldInfo.GetValue(targetObject);
            if (bitset == null) return;

            Type bitsetType = bitset.GetType();
            Type enumType = bitsetType.GetGenericArguments()[0];
            Array enumValues = Enum.GetValues(enumType);
            string[] enumNames = Enum.GetNames(enumType);

            int previousMask = GetMask(enumValues, (IEnumerable) bitset);
            int mask = EditorGUI.MaskField(position, label, previousMask, enumNames);
            if (previousMask == mask) return;

            Undo.RecordObject(targetObject, "Set enum bit mask");
            bitsetType.GetMethod("Clear").Invoke(bitset, null);

            MethodInfo BitSet_Add = bitsetType.GetMethod("Add");
            var args = new object[1];
            foreach (int bitIndex in Commons.EnumerateSetBits(mask))
            {
                args[0] = enumValues.GetValue(bitIndex);
                BitSet_Add.Invoke(bitset, args);
            }

            EditorUtility.SetDirty(targetObject);
        }

        static int GetMask(Array enumValues, IEnumerable bitset)
        {
            var mask = 0;
            foreach (object value in bitset)
            {
                int index = Array.IndexOf(enumValues, value);
                Debug.Assert(index >= 0, $"Couldn't find enum with value {value}");
                mask |= 1 << index;
            }
            return mask;
        }
    }
}
