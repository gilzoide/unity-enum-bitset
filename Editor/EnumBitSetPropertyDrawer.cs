using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gilzoide.EnumBitSet.Editor
{
    [CustomPropertyDrawer(typeof(EnumBitSet32<>))]
    [CustomPropertyDrawer(typeof(EnumBitSet64<>))]
    public class EnumBitSetPropertyDrawer : PropertyDrawer
    {
        private readonly Vector2 BUTTON_PADDING = new Vector2(4, 0);
            
        private bool _isShowingChildren;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            List<string> currentEnums = EnumBitSetEditorUtility.GetSerializedEnumNames(property).ToList();
            string joinedNames = string.Join(" | ", currentEnums);
            label.text += " [" + joinedNames + "]";
            label.tooltip = joinedNames;
            
            float labelHeight = EditorGUI.GetPropertyHeight(SerializedPropertyType.String, label);
            var entryRect = new Rect(position.x, position.y, position.width, labelHeight);
            _isShowingChildren = EditorGUI.PropertyField(entryRect, property, label); 
            if (!_isShowingChildren)
            {
                return;
            }
            
            EditorGUI.indentLevel++;
            // "Select All" | "Unselect All" buttons
            entryRect.y += entryRect.height;
            var buttonRect = new Rect(entryRect.position, entryRect.size * new Vector2(0.5f, 1f) - BUTTON_PADDING);
            bool selectAll = GUI.Button(buttonRect, "Select All");
            buttonRect.x += entryRect.width * 0.5f + BUTTON_PADDING.x;
            bool unselectAll = GUI.Button(buttonRect, "Unselect All");

            // Draw every enum, returning the names of the marked ones
            List<(string, int)> newEnums = GetEnumType().GetEnumNames().Select((name, i) => (name, i))
                .Where(entry =>
                {
                    var content = new GUIContent(entry.name);
                    entryRect.y += entryRect.height;
                    entryRect.height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Boolean, content);
                    bool hasValue = !unselectAll && (selectAll || currentEnums.Contains(entry.name));
                    return EditorGUI.Toggle(entryRect, content, hasValue);
                }
            ).ToList();
            EditorGUI.indentLevel--;
            
            SetSerializedEnums(property, newEnums);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUI.GetPropertyHeight(SerializedPropertyType.String, label);
            
            if (!_isShowingChildren)
            {
                return height;
            }

            return height
                + height  // "Select All" | "Unselect All" buttons
                + GetEnumType().GetEnumNames()
                    .Sum(name => EditorGUI.GetPropertyHeight(SerializedPropertyType.Boolean, new GUIContent(name)));
        }

        private Type GetEnumType()
        {
            Type[] genericArgs = fieldInfo.FieldType.GetGenericArgumentsOfBase(typeof(EnumBitSet<,>));
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
