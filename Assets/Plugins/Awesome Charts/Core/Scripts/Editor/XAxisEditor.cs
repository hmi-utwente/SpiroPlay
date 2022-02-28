using UnityEditor;
using UnityEngine;

namespace AwesomeCharts {
    [CustomPropertyDrawer(typeof(XAxis))]
    class XAxisEditor : AxisBaseEditor {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            base.OnGUI(position, property, label);
        }

        protected override float SetupLineProperties(SerializedProperty property, Rect contentRect, float positionY) {
            EditorGUI.PropertyField(new Rect(contentRect.x, positionY, contentRect.width / 3, contentRect.height),
            property.FindPropertyRelative("lineThickness"), new GUIContent("width"));
            EditorGUI.PropertyField(new Rect(contentRect.x + contentRect.width / 3, positionY, contentRect.width / 3, contentRect.height),
            property.FindPropertyRelative("linesCount"), new GUIContent("count"));
            EditorGUI.PropertyField(new Rect(contentRect.x + contentRect.width * 2 / 3, positionY, contentRect.width / 3, contentRect.height),
            property.FindPropertyRelative("lineColor"), new GUIContent("color"));

            positionY += LINE_HEIGHT;

            EditorGUI.PropertyField(new Rect(contentRect.x, positionY, contentRect.width / 3, contentRect.height),
            property.FindPropertyRelative("lineStep"), new GUIContent("step", "Defines value space between lines. Use instead of count property in case of dynamic lines count."));

            return positionY;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return base.GetPropertyHeight(property, label) + LINE_HEIGHT;
        }
    }
}
