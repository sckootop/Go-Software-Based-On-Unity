using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IntOptionsAttribute))]
public class IntOptionsDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        IntOptionsAttribute optionsAttribute = (IntOptionsAttribute)attribute;

        if (property.propertyType == SerializedPropertyType.Integer)
        {
            EditorGUI.BeginChangeCheck();
            int currentValue = property.intValue;
            int index = Mathf.Max(0, System.Array.IndexOf(optionsAttribute.Options, currentValue));
            index = EditorGUI.Popup(position, label.text, index, optionsAttribute.Options.Select(x => x.ToString()).ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = optionsAttribute.Options[index];
            }
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use IntOptions with int.");
        }
    }
}
