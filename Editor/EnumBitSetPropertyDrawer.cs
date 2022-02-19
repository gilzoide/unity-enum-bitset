using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EnumBitSet
{
    public abstract class EnumBitSetPropertyDrawer : PropertyDrawer
    {
        private bool _showFold;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type enumType = GetEnumType();
            var currentMask = GetCurrentMask(property, enumType);
            label.text += " [" + string.Join(" | ", EnumerableToString((IEnumerable) currentMask)) + "]";
            
            var labelHeight = EditorGUI.GetPropertyHeight(SerializedPropertyType.String, label);
            var entryRect = new Rect(position.x, position.y, position.width, labelHeight);
            _showFold = EditorGUI.Foldout(entryRect, _showFold, label);
            if (!_showFold) return;

            var maskType = currentMask.GetType();
            var Set_Contains = GetMethod(maskType, "Contains");

            var enumListType = typeof(List<>).MakeGenericType(enumType);
            var List_Add = GetMethod(enumListType, "Add");
            var setValuesList = Activator.CreateInstance(enumListType);

            EditorGUI.indentLevel++;
            foreach (var (name, value) in GetEnumEntries())
            {
                var content = new GUIContent(name);
                var hasValue = Convert.ToBoolean(Set_Contains.Invoke(currentMask, new[] { value }));
                
                entryRect.y += entryRect.height;
                entryRect.height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Boolean, content);
                if (EditorGUI.Toggle(entryRect, content, hasValue))
                {
                    List_Add.Invoke(setValuesList, new[] { value });
                }
            }
            EditorGUI.indentLevel--;

            var Set_SetEquals = GetMethod(maskType, "SetEquals");
            if (Convert.ToBoolean(Set_SetEquals.Invoke(currentMask, new[] { setValuesList }))) return;
            
            Undo.RecordObjects(property.serializedObject.targetObjects, "Set enum bit mask");
            SetMask(property, (IEnumerable) setValuesList);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUI.GetPropertyHeight(SerializedPropertyType.String, label);

            if (_showFold)
            {
                foreach (var (name, _) in GetEnumEntries())
                {
                    height += EditorGUI.GetPropertyHeight(SerializedPropertyType.Boolean, new GUIContent(name));
                }
            }
            
            return height;
        }

        protected abstract object GetCurrentMask(SerializedProperty property, Type enumType);

        protected abstract void SetMask(SerializedProperty property, IEnumerable enumSet);

        private Type GetEnumType()
        {
            return fieldInfo.FieldType.GetGenericArguments()[0];
        }

        private List<EnumEntry> GetEnumEntries()
        {
            Type enumType = GetEnumType();
            Array values = Enum.GetValues(enumType);
            string[] names = Enum.GetNames(enumType);

            var list = new List<EnumEntry>(values.Length);
            for (var i = 0; i < values.Length; i++)
            {
                list.Add(new EnumEntry
                {
                    Name = names[i],
                    Value = values.GetValue(i),
                });
            }
            return list;
        }

        private static MethodInfo GetMethod(Type type, string name)
        {
            var info = type.GetMethod(name);
            Debug.Assert(info != null, $"FIXME: {type}.{name} is null");
            return info;
        }

        private static IEnumerable<string> EnumerableToString(IEnumerable enumerable)
        {
            foreach (var obj in enumerable)
            {
                yield return obj.ToString();
            }
        }


        private class EnumEntry
        {
            public string Name;
            public object Value;

            public void Deconstruct(out string name, out object value)
            {
                name = Name;
                value = Value;
            }
        }
    }

    [CustomPropertyDrawer(typeof(EnumBitSet32<>))]
    public class EnumBitSet32PropertyDrawer : EnumBitSetPropertyDrawer
    {
        protected override object GetCurrentMask(SerializedProperty property, Type enumType)
        {
            var mask = property.FindPropertyRelative("_bitMask").intValue;
            var bitmaskType = typeof(EnumBitMask32<>).MakeGenericType(enumType);
            return Activator.CreateInstance(bitmaskType, mask);
        }

        protected override void SetMask(SerializedProperty property, IEnumerable enumSet)
        {
            var mask = 0;
            foreach (var value in enumSet)
            {
                mask |= 1 << Convert.ToInt32(value);
            }
            property.FindPropertyRelative("_bitMask").intValue = mask;
        }
    }
    
    
    [CustomPropertyDrawer(typeof(EnumBitSet64<>))]
    public class EnumBitSet64PropertyDrawer : EnumBitSetPropertyDrawer
    {
        protected override object GetCurrentMask(SerializedProperty property, Type enumType)
        {
            var mask = property.FindPropertyRelative("_bitMask").longValue;
            var bitmaskType = typeof(EnumBitMask64<>).MakeGenericType(enumType);
            return Activator.CreateInstance(bitmaskType, mask);
        }

        protected override void SetMask(SerializedProperty property, IEnumerable enumSet)
        {
            var mask = 0L;
            foreach (var value in enumSet)
            {
                mask |= 1L << Convert.ToInt32(value);
            }
            property.FindPropertyRelative("_bitMask").longValue = mask;
        }
    }
}
