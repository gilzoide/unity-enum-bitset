using System.Collections.Generic;
using UnityEditor;

namespace Gilzoide.EnumBitSet.Editor
{
    public static class EnumBitSetEditorUtility
    {
        public static IEnumerable<string> GetSerializedEnumNames(SerializedProperty bitsetProperty)
        {
            SerializedProperty serializedEnums = bitsetProperty.FindPropertyRelative("_serializedEnums");
            for (var i = 0; i < serializedEnums.arraySize; i++)
            {
                string name = serializedEnums.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue;
                yield return name;
            }
        }
    }
}