using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gilzoide.EnumBitSet.Editor
{
    [CustomPropertyDrawer(typeof(EnumBitSet32<>))]
    [CustomPropertyDrawer(typeof(EnumBitSet64<>))]
    public class EnumBitSetPropertyDrawer : PropertyDrawer
    {
        private readonly Vector2 BUTTON_PADDING = new Vector2(4, 0);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            List<string> currentEnums = new List<string>(EnumBitSetEditorUtility.GetSerializedEnumNames(property));
            string joinedNames = string.Join(" | ", currentEnums);
            label.text += " [" + joinedNames + "]";
            label.tooltip = joinedNames;

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float space = EditorGUIUtility.standardVerticalSpacing;
            
            var entryRect = new Rect(position.x, position.y, position.width, lineHeight);
            property.isExpanded = EditorGUI.PropertyField(entryRect, property, label); 
            if (!property.isExpanded)
            {
                return;
            }
            
            EditorGUI.indentLevel++;
            // "Select All" | "Unselect All" buttons
            entryRect.y += space + entryRect.height;
            var buttonRect = new Rect(entryRect.position, entryRect.size * new Vector2(0.5f, 1f) - BUTTON_PADDING);
            bool selectAll = GUI.Button(buttonRect, "Select All");
            buttonRect.x += entryRect.width * 0.5f + BUTTON_PADDING.x;
            bool unselectAll = GUI.Button(buttonRect, "Unselect All");

            // Draw every enum, saving the names of the marked ones
            string[] enumNames = GetEnumType().GetEnumNames();
            List<(string, int)> markedEnums = new List<(string, int)>();
            for (int i = 0; i < enumNames.Length; i++)
            {
                string name = enumNames[i];
                entryRect.y += space + entryRect.height;
                bool hasValue = !unselectAll && (selectAll || currentEnums.Contains(name));
                if (EditorGUI.Toggle(entryRect, name, hasValue))
                {
                    markedEnums.Add((name, i));
                }
            }
            EditorGUI.indentLevel--;
            
            SetSerializedEnums(property, markedEnums);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float space = EditorGUIUtility.standardVerticalSpacing;

            float height = lineHeight;
            if (property.isExpanded)
            {
                height += space + lineHeight;  // "Select All" | "Unselect All" buttons
                height += GetEnumType().GetEnumNames().Length * (space + lineHeight);
            }
            return height;
        }

        private Type GetEnumType()
        {
            Type propertyType = fieldInfo.FieldType;
            if (propertyType.IsArray)
            {
                propertyType = propertyType.GetElementType();
            }
            else if (typeof(IList).IsAssignableFrom(propertyType))
            {
                propertyType = propertyType.GetGenericArguments()[0];
            }

            Type[] genericArgs = propertyType.GetGenericArgumentsOfBase(typeof(EnumBitSet<,>));
            if (genericArgs?.Length > 0)
            {
                return genericArgs[0];
            }

            throw new Exception("EnumBitSetPropertyDrawer can only be drawer for types inheriting from EnumBitSet<,>");
        }

        private void SetSerializedEnums(SerializedProperty baseProperty, IEnumerable<(string, int)> entries)
        {
            SerializedProperty serializedEnums = baseProperty.FindPropertyRelative("_serializedEnums");
            serializedEnums.ClearArray();

            Array enumValues = GetEnumType().GetEnumValues();

            var serializedIndex = 0;
            foreach ((string name, int index) in entries)
            {
                serializedEnums.InsertArrayElementAtIndex(serializedIndex);
                SerializedProperty newEnum = serializedEnums.GetArrayElementAtIndex(serializedIndex);
                newEnum.FindPropertyRelative("Name").stringValue = name;
                newEnum.FindPropertyRelative("Value").intValue = Convert.ToInt32(enumValues.GetValue(index));
                serializedIndex++;
            }
        }
    }
}
