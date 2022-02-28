using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace AwesomeCharts {
    [ExecuteInEditMode]
    [RequireComponent(typeof(UILineRenderer))]
    abstract public class AxisRendererBase : ACMonoBehaviour {

        private readonly int DASH_LENGTH = Defaults.AXIS_RENDERER_LINE_DASH_LENGTH;
        private readonly int DASH_SPACE = Defaults.AXIS_RENDERER_LINE_DASH_SPACE;

        protected abstract AxisBase GetAxis();

        protected abstract Vector2[] GetLinePoints();

        protected abstract Vector3[] GetLabelPositions();

        protected abstract float[] GetLineValues();

        protected abstract Vector2 GetLabelPivot();

        public float axisMinValue = 0f;

        public float axisMaxValue = 100f;

        private List<Text> labels = new List<Text>();

        public List<string> customLabelValues;

        private ViewCreator viewCreator = new ViewCreator();

        private IValueFormatter valueFormatter;

        private UILineRenderer lineRenderer;

        private void Awake() {
            Setup();
            Invalidate();
        }

        protected virtual void ApplyLabelTransformation(Text label) { }

        protected virtual IValueFormatter ProvideValueFormatter() {
            return new SimpleValueFormatter();
        }

        protected float CalculateLineSpacing(float totalSize) {
            if (GetAxis().LinesCount < 2)
                return 0.0f;
            else
                return totalSize / (GetAxis().LinesCount - 1);
        }

        private void Setup() {
            valueFormatter = ProvideValueFormatter();
            lineRenderer = GetComponent<UILineRenderer>();
            lineRenderer.lineList = true;
        }

        public void Invalidate() {
            if (GetAxis() == null || valueFormatter == null) {
                return;
            }

            DrawLines();
            DrawLabels();
        }

        private void DrawLines() {
            lineRenderer.lineThickness = GetAxis().LineThickness;
            lineRenderer.color = GetAxis().LineColor;
            lineRenderer.Points = GetAxis().ShouldDrawLines ? GetAxisLinesPoints() : new Vector2[0];
        }

        private Vector2[] GetAxisLinesPoints() {
            Vector2[] linePoints = GetLinePoints();
            return GetAxis().DashedLine ? GetDashLinePoints(linePoints) : linePoints;
        }

        private Vector2[] GetDashLinePoints(Vector2[] linesPoints) {
            List<Vector2> result = new List<Vector2>();

            for (int i = 0; i < linesPoints.Length / 2; i++) {
                result.AddRange(GetDashedLinePoints(linesPoints[i * 2],
                                                          linesPoints[(i * 2) + 1]));
            }

            return result.ToArray();
        }

        private List<Vector2> GetDashedLinePoints(Vector2 start, Vector2 end) {
            List<Vector2> result = new List<Vector2>();

            Vector2 differenceVector = end - start;
            Vector2 dv, sv;

            if (differenceVector.x > 0f) {
                dv = new Vector2(DASH_LENGTH, 0f);
                sv = new Vector2(DASH_SPACE, 0f);
            } else {
                dv = new Vector2(0f, DASH_LENGTH);
                sv = new Vector2(0f, DASH_SPACE);
            }

            Vector2 currentVector = start;
            int dashes = (int)Vector2.Distance(start, end) / (DASH_LENGTH + DASH_SPACE);
            for (int i = 0; i < dashes; i++) {
                result.Add(new Vector2(currentVector.x, currentVector.y));
                result.Add(currentVector + dv);
                currentVector += (dv + sv);
            }

            result.Add(new Vector2(currentVector.x, currentVector.y));
            result.Add(new Vector2(end.x, end.y));

            return result;
        }

        private void DrawLabels() {
            ShowHideLabels(GetAxis().ShouldDrawLabels);
            if (!GetAxis().ShouldDrawLabels) {
                return;
            }

            AxisBase axisInfo = GetAxis();
            Vector3[] labelPositions = GetLabelPositions();
            string[] labelTextValues = GetLabelsTextValues();

            UpdateLabelsInstances(labelPositions.Length);

            for (int i = 0; i < labelPositions.Length; i++) {
                Text labelText = labels[i];
                labelText.transform.localPosition = labelPositions[i];
                labelText.text = labelTextValues[i];
                labelText.color = axisInfo.LabelColor;
                labelText.fontSize = axisInfo.LabelSize;
                labelText.fontStyle = axisInfo.LabelFontStyle;
                if (axisInfo.LabelFont != null) {
                    labelText.font = axisInfo.LabelFont;
                }

                ApplyLabelTransformation(labelText);
            }
        }

        private void ShowHideLabels(bool shown) {
            labels.ForEach(label => {
                label.enabled = shown;
            });
        }

        private void UpdateLabelsInstances(int requiredCount) {
            int currentLabelsCount = labels.Count;

            // Add missing labels
            int missingLabelsCount = requiredCount - currentLabelsCount;
            while (missingLabelsCount > 0) {
                Text labelText = viewCreator.InstantiateBottomLabel("Label", transform, GetLabelPivot());
                labels.Add(labelText);
                missingLabelsCount--;
            }

            // Remove redundant labels
            int redundantLabelsCount = currentLabelsCount - requiredCount;
            while (redundantLabelsCount > 0) {
                Text target = labels[labels.Count - 1];
                DestroyDelayed(target.gameObject);
                labels.Remove(target);
                redundantLabelsCount--;
            }
        }

        private string[] GetLabelsTextValues() {
            float[] labelValues = GetLineValues();
            string[] result = new string[labelValues.Length];
            
            for (int i = 0; i < labelValues.Length; i++) {
                if (customLabelValues != null && customLabelValues.Count > i) {
                    result[i] = customLabelValues[i];
                } else if (customLabelValues == null || customLabelValues.Count == 0) {
                    result[i] = valueFormatter.FormatAxisValue(
                        labelValues[i],
                        labelValues[0],
                        labelValues[labelValues.Length - 1]);
                } else {
                    break;
                }
            }

            return result;
        }
    }
}