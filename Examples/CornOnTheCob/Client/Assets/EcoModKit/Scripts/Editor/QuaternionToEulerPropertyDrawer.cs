using UnityEditor;
using UnityEngine;
 
[CustomPropertyDrawer(typeof(QuaternionToEulerAttribute))]
public class QuaternionToEulerPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var euler = property.quaternionValue.eulerAngles;
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.BeginChangeCheck();
        euler = EditorGUI.Vector3Field(position, label, euler);
        if (EditorGUI.EndChangeCheck())
            property.quaternionValue = Quaternion.Euler(euler);
        EditorGUI.EndProperty();
    }
}