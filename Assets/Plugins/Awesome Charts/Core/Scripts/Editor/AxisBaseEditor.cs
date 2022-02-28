using System;
using UnityEditor;
using UnityEngine;

namespace AwesomeCharts {
    abstract class AxisBaseEditor : PropertyDrawer {

        protected const float LINE_HEIGHT = 20f;

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            label = EditorGUI.BeginProperty (position, label, property);
            EditorGUI.PrefixLabel (position, label);

            Rect labelRect = new Rect (position.x + 15f, position.y, 70f, EditorGUIUtility.singleLineHeight);
            Rect contentRect = new Rect (labelRect.x + labelRect.width, labelRect.y, position.width - labelRect.width - 15f, EditorGUIUtility.singleLineHeight);

            float currentY = labelRect.y;
            currentY += LINE_HEIGHT;

            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 45f;

            EditorGUI.LabelField (new Rect (labelRect.x, currentY, labelRect.width, labelRect.height), "Lines: ");

            currentY = SetupLineProperties (property, contentRect, currentY);
            currentY += LINE_HEIGHT;

            EditorGUI.LabelField (new Rect (labelRect.x, currentY, labelRect.width, labelRect.height), "Axis value: ");

            currentY = SetupAxisValuesProperties (property, contentRect, currentY);
            currentY += LINE_HEIGHT;

            EditorGUI.LabelField (new Rect (labelRect.x, currentY, labelRect.width, labelRect.height), "Labels: ");

            currentY = SetupLabelsProperties (property, contentRect, currentY);
            currentY += LINE_HEIGHT;

            EditorGUI.LabelField (new Rect (labelRect.x, currentY, labelRect.width, labelRect.height), "Draw: ");
            currentY = SetupDrawingProperties (property, contentRect, currentY);

            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.EndProperty ();
        }

        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
            return 8 * LINE_HEIGHT;
        }

        protected virtual float SetupLineProperties (SerializedProperty property, Rect contentRect, float positionY) {
            EditorGUI.PropertyField (new Rect (contentRect.x, positionY, contentRect.width / 3, contentRect.height),
                property.FindPropertyRelative ("lineThickness"), new GUIContent ("width"));
            EditorGUI.PropertyField (new Rect (contentRect.x + contentRect.width / 3, positionY, contentRect.width / 3, contentRect.height),
                property.FindPropertyRelative ("linesCount"), new GUIContent ("count"));
            EditorGUI.PropertyField (new Rect (contentRect.x + contentRect.width * 2 / 3, positionY, contentRect.width / 3, contentRect.height),
                property.FindPropertyRelative ("lineColor"), new GUIContent ("color"));

            return positionY;
        }

        protected virtual float SetupAxisValuesProperties (SerializedProperty property, Rect contentRect, float positionY) {
            EditorGUI.PropertyField (new Rect (contentRect.x, positionY, contentRect.width / 3, contentRect.height),
                property.FindPropertyRelative ("minAxisValue"), new GUIContent ("min"));
            EditorGUI.PropertyField (new Rect (contentRect.x + contentRect.width / 3, positionY, contentRect.width / 3, contentRect.height),
                property.FindPropertyRelative ("autoAxisMinValue"), new GUIContent ("auto"));

            positionY += LINE_HEIGHT;

            EditorGUI.PropertyField (new Rect (contentRect.x, positionY, contentRect.width / 3, contentRect.height),
                property.FindPropertyRelative ("maxAxisValue"), new GUIContent ("max"));
            EditorGUI.PropertyField (new Rect (contentRect.x + contentRect.width / 3, positionY, contentRect.width / 3, contentRect.height),
                property.FindPropertyRelative ("autoAxisMaxValue"), new GUIContent ("auto"));

            return positionY;
        }

        protected virtual float SetupLabelsProperties (SerializedProperty property, Rect contentRect, float positionY) {
            EditorGUI.PropertyField (new Rect (contentRect.x, positionY, contentRect.width / 3, contentRect.height),
                property.FindPropertyRelative ("labelSize"), new GUIContent ("size"));
            EditorGUI.PropertyField (new Rect (contentRect.x + contentRect.width / 3, positionY, contentRect.width * 2 / 3, contentRect.height),
                property.FindPropertyRelative ("labelFont"), new GUIContent ("font"));

            positionY += LINE_HEIGHT;

            EditorGUI.PropertyField (new Rect (contentRect.x, positionY, contentRect.width / 3, contentRect.height),
                property.FindPropertyRelative ("labelColor"), new GUIContent ("color"));
            EditorGUI.PropertyField (new Rect (contentRect.x + contentRect.width / 3, positionY, contentRect.width * 2 / 3, contentRect.height),
                property.FindPropertyRelative ("labelFontStyle"), new GUIContent ("style"));

            positionY += LINE_HEIGHT;

            EditorGUI.PropertyField (new Rect (contentRect.x, positionY, contentRect.width / 3, contentRect.height),
                property.FindPropertyRelative ("labelMargin"), new GUIContent ("margin"));

            return positionY;
        }

        protected virtual float SetupDrawingProperties (SerializedProperty property, Rect contentRect, float positionY) {
            EditorGUI.PropertyField (new Rect (contentRect.x, positionY, contentRect.width / 3, contentRect.height),
                property.FindPropertyRelative ("shouldDrawLines"), new GUIContent ("lines"));
            EditorGUI.PropertyField (new Rect (contentRect.x + contentRect.width / 3, positionY, contentRect.width / 3, contentRect.height),
                property.FindPropertyRelative ("shouldDrawLabels"), new GUIContent ("labels"));
            EditorGUI.PropertyField (new Rect (contentRect.x + contentRect.width * 2 / 3, positionY, contentRect.width / 3, contentRect.height),
                property.FindPropertyRelative ("dashedLine"), new GUIContent ("dashed"));

            return positionY;
        }
    }
}