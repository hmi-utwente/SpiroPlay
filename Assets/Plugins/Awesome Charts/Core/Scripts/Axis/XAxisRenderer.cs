using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AwesomeCharts {
    public class XAxisRenderer : AxisRendererBase {

        private XAxis axis;

        public XAxis Axis {
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
            float[] xPositions = GetXPositions();
            float parentHeight = gameObject.GetComponent<RectTransform>().sizeDelta.y;

            Vector2[] linePoints = new Vector2[xPositions.Length * 2];
            for (int i = 0; i < xPositions.Length; i++) {
                linePoints[i * 2] = new Vector2(xPositions[i], 0);
                linePoints[(i * 2) + 1] = new Vector2(xPositions[i], parentHeight);
            }

            return linePoints;
        }

        protected virtual float[] GetXPositions() {
            if (Axis.LinesCount == 0 && Axis.LineStep < 1)
                return new float[0];

            float parentWidth = gameObject.GetComponent<RectTransform>().sizeDelta.x;
            float lineSpacing = 0;
            int linesCount = 0;
            if (Axis.LineStep > 0) {
                float valuePerPoint = parentWidth / axisMaxValue;
                lineSpacing = Axis.LineStep * valuePerPoint;
                linesCount = (int)(Axis.MaxAxisValue / Axis.LineStep) + 1;
            } else {
                lineSpacing = CalculateLineSpacing(parentWidth);
                linesCount = Axis.LinesCount;
            }

            float[] positions = new float[linesCount];
            for (int i = 0; i < linesCount; i++) {
                positions[i] = lineSpacing * i;
            }

            return positions;
        }

        protected override Vector3[] GetLabelPositions() {
            float[] xPositions = GetXPositions();
            Vector3[] result = new Vector3[xPositions.Length];
            for (int i = 0; i < xPositions.Length; i++) {
                result[i] = new Vector3(xPositions[i], -Axis.LabelMargin, 0);
            }
            return result;
        }

        protected override float[] GetLineValues() {
            float[] xPositions = GetXPositions();
            float[] result = new float[xPositions.Length];
            float parentWidth = gameObject.GetComponent<RectTransform>().sizeDelta.x;
            float xDelta = axisMaxValue - axisMinValue;
            for (int i = 0; i < xPositions.Length; i++) {
                float lineValue = axisMinValue + ((xPositions[i] / parentWidth) * xDelta);
                result[i] = lineValue;
            }

            return result;
        }

        protected override Vector2 GetLabelPivot() {
            return PivotValue.TOP_MIDDLE;
        }

        protected override void ApplyLabelTransformation(Text label) {
            label.alignment = TextAnchor.UpperCenter;
            label.maskable = false;
        }
    }
}