using UnityEditor;
using UnityEngine;

namespace AwesomeCharts {
    [CustomPropertyDrawer(typeof(YAxis))]
    class YAxisEditor : AxisBaseEditor {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            base.OnGUI(position, property, label);
        }
    }
}
