using System;
using UnityEngine;
using UnityEngine.UI;

namespace AwesomeCharts {
    public class YAxisRenderer : AxisRendererBase {

        private YAxis axis;
        
        public YAxis Axis {
            get { return axis; }
            set {
                axis = value;
                Invalidate();
            }
        }

        protected override AxisBase GetAxis() {
            return Axis;
        }

        protected override Vector2[] GetLinePoints() {
            float[] yPositions = GetYPositions();
            float parentWidth = gameObject.GetComponent<RectTransform>().sizeDelta.x;

            Vector2[] result = new Vector2[yPositions.Length * 2];
            for (int i = 0; i < yPositions.Length; i++) {
                result[i * 2] = new Vector2(0, yPositions[i]);
                result[(i * 2) + 1] = new Vector2(parentWidth, yPositions[i]);
            }
            return result;
        }

        protected override Vector3[] GetLabelPositions() {
            float[] yPositions = GetYPositions();

            Vector3[] result = new Vector3[yPositions.Length];
            for (int i = 0; i < yPositions.Length; i++) {
                result[i] = new Vector3(-Axis.LabelMargin, yPositions[i], 0);
            }
            return result;
        }

        protected override Vector2 GetLabelPivot() {
            return PivotValue.MIDDLE_RIGHT;
        }

        protected virtual float[] GetYPositions() {
            if (Axis.LinesCount <= 0) {
                return new float[0];
            }

            float parentHeight = gameObject.GetComponent<RectTransform>().sizeDelta.y;
            float lineSpacing = CalculateLineSpacing(parentHeight);
            float[] result = new float[Axis.LinesCount];
            for (int i = 0; i < Axis.LinesCount; i++) {
                result[i] = lineSpacing * i;
            }

            return result;
        }

        protected override float[] GetLineValues() {
            float[] yPositions = GetYPositions();
            float[] result = new float[yPositions.Length];
            float parentHeight = gameObject.GetComponent<RectTransform>().sizeDelta.y;
            float yDelta = axisMaxValue - axisMinValue;
            for (int i = 0; i < yPositions.Length; i++) {
                float lineValue = axisMinValue + ((yPositions[i] / parentHeight) * yDelta);
                result[i] = lineValue;
            }

            return result;
        }

        protected override void ApplyLabelTransformation(Text label) {
            label.alignment = TextAnchor.MiddleRight;
        }
    }
}