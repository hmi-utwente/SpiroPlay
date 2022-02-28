using UnityEngine;

namespace AwesomeCharts {
    public class XAxisBarChartRenderer : XAxisRenderer {

        public BarCharPositioner barChartPositioner;

        protected override IValueFormatter ProvideValueFormatter() {
            return new XAxisBarChartFormatter();
        }

        protected override float[] GetXPositions() {
            if (barChartPositioner == null)
                return new float[0];

            int axisFirstVisibleIndex = (int)barChartPositioner.axisBounds.XMin;

            float[] positions = new float[barChartPositioner.GetVisibleEntriesRange()];
            for (int i = 0; i < positions.Length; i++) {
                positions[i] = barChartPositioner.GetBarCenterPosition(i + axisFirstVisibleIndex).x;
            }

            return positions;
        }

        protected override float[] GetLineValues() {
            if (barChartPositioner == null)
                return new float[0];

            int axisFirstVisibleIndex = (int)barChartPositioner.axisBounds.XMin;

            float[] result = new float[barChartPositioner.GetVisibleEntriesRange()];
            for (int i = 0; i < result.Length; i++) {
                result[i] = i + axisFirstVisibleIndex;
            }

            return result;
        }

        protected override Vector2[] GetLinePoints() {
            return new Vector2[0];
        }
    }
}