using System;
using UnityEngine;

namespace AwesomeCharts {

    public abstract class AxisBaseChart<T> : BaseChart<T> where T : AxisChartData {
        [SerializeField]
        private XAxis xAxis;

        [SerializeField]
        private YAxis yAxis;

        public XAxis XAxis {
            get { return xAxis; }
            set {
                xAxis = value;
                SetDirty ();
            }
        }

        public YAxis YAxis {
            get { return yAxis; }
            set {
                yAxis = value;
                SetDirty ();
            }
        }

        protected XAxisRenderer xAxisRenderer;
        protected YAxisRenderer yAxisRenderer;

        protected virtual void OnUpdateAxis () { }

        protected virtual void Awake () {
            if (xAxis == null)
                xAxis = new XAxis ();

            if (yAxis == null)
                yAxis = new YAxis ();
        }

        protected override void OnInstantiateViews () {
            base.OnInstantiateViews ();

            xAxisRenderer = InstantiateXAxisRenderer ();
            xAxisRenderer.transform.SetSiblingIndex (0);
            yAxisRenderer = InstantiateYAxisRenderer ();
            yAxisRenderer.transform.SetSiblingIndex (1);
        }

        protected virtual XAxisRenderer InstantiateXAxisRenderer () {
            return viewCreator.InstantiateXAxisRenderer ("XAxis", contentView.transform, PivotValue.BOTTOM_LEFT);
        }

        protected virtual YAxisRenderer InstantiateYAxisRenderer () {
            return viewCreator.InstantiateYAxisRenderer ("YAxis", contentView.transform, PivotValue.BOTTOM_LEFT);
        }

        protected override void OnUpdateViewsSize (Vector2 size) {
            base.OnUpdateViewsSize (size);

            xAxisRenderer.GetComponent<RectTransform> ().sizeDelta = size;
            yAxisRenderer.GetComponent<RectTransform> ().sizeDelta = size;
        }

        protected override void OnDrawChartContent () {
            base.OnDrawChartContent ();
            UpdateAxis ();
        }

        private void UpdateAxis () {
            if (GetChartData () == null)
                return;

            AxisBounds axisBounds = GetAxisBounds ();

            xAxisRenderer.Axis = XAxis;
            xAxisRenderer.axisMinValue = axisBounds.XMin;
            xAxisRenderer.axisMaxValue = axisBounds.XMax;
            xAxisRenderer.customLabelValues = GetChartData ().CustomLabels;

            yAxisRenderer.Axis = YAxis;
            yAxisRenderer.axisMinValue = axisBounds.YMin;
            yAxisRenderer.axisMaxValue = axisBounds.YMax;

            OnUpdateAxis ();
            xAxisRenderer.Invalidate ();
            yAxisRenderer.Invalidate ();
        }

        protected AxisBounds GetAxisBounds () {
            float xMin = XAxis.AutoAxisMinValue ? GetChartData ().GetMinPosition () : XAxis.MinAxisValue;
            float xMax = XAxis.AutoAxisMaxValue ? GetChartData ().GetMaxPosition () : XAxis.MaxAxisValue;
            float yMin = YAxis.AutoAxisMinValue ? GetClosestRoundValue (GetChartData ().GetMinValue (), GetChartData ().GetMinValue () < 0) :
                YAxis.MinAxisValue;
            float yMax = YAxis.AutoAxisMaxValue ? GetClosestRoundValue (GetChartData ().GetMaxValue (), GetChartData ().GetMaxValue () > 0) :
                YAxis.MaxAxisValue;

            return new AxisBounds (xMin, xMax, yMin, yMax);
        }

        private int GetClosestRoundValue (float value, bool up) {
            if (value == 0)
                return 0;

            float valueRoundDifference = CalculateRoundingDifferenceForValue (value);
            if (up) {
                return (int) (value + valueRoundDifference);
            } else {
                return (int) (value - valueRoundDifference);
            }
        }

        private float CalculateRoundingDifferenceForValue (float value) {
            int signMultiplyer = value >= 0 ? 1 : -1;
            float currentValue = Math.Abs (value * 1.1f);
            float log10 = Mathf.FloorToInt (Mathf.Log10 (currentValue) + 1);
            currentValue = Mathf.Ceil (currentValue);
            if (log10 > 2) {
                currentValue = ((int) (currentValue / Mathf.Pow (10, log10 - 2)) + 1) * Mathf.Pow (10, log10 - 2);
            } else if (log10 >= 1) {
                currentValue = ((int) (currentValue / Mathf.Pow (10, log10 - 1)) + 1) * Mathf.Pow (10, log10 - 1);
            }
            return (currentValue - Math.Abs(value)) * signMultiplyer;
        }
    }
}