#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SerializebleRangeInt))]
public class SerializableRangeIntDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty startProp = property.FindPropertyRelative("_start");
        SerializedProperty endProp = property.FindPropertyRelative("_end");

        EditorGUI.BeginProperty(position, label, property);

        float partWidth = position.width / 3f; 
        Rect propertyName = new Rect(position.x, position.y, partWidth, position.height);
        Rect startRect = new Rect(position.x + partWidth, position.y, partWidth, position.height); 
        Rect endRect = new Rect(position.x + partWidth * 2 , position.y, partWidth, position.height);


        // Рисуем поля
        EditorGUI.LabelField(propertyName, new GUIContent(property.displayName));
        EditorGUIUtility.labelWidth = 35;
        EditorGUI.PropertyField(startRect, startProp, new GUIContent("Start"));
        EditorGUIUtility.labelWidth = 25;
        EditorGUI.PropertyField(endRect, endProp, new GUIContent("End"));

        // Применяем ограничение (например, минимальное значение 0)
        if (startProp.intValue <= 0) startProp.intValue = 1;
        if (endProp.intValue <= startProp.intValue) endProp.intValue = startProp.intValue + 1;

        EditorGUI.EndProperty();
    }
}
#endif