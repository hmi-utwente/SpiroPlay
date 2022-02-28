using UnityEngine;

namespace AwesomeCharts {
    public class LineChartController2 : MonoBehaviour {

        public LineChart chart;

        private void Start() {
            ConfigChart();
            AddChartData();
        }

        private void ConfigChart() {
            chart.Config.ValueIndicatorSize = 17;

            chart.XAxis.DashedLine = true;
            chart.XAxis.LineThickness = 1;
            chart.XAxis.LabelColor = Color.white;
            chart.XAxis.LabelSize = 18;

            chart.YAxis.DashedLine = true;
            chart.YAxis.LineThickness = 1;
            chart.YAxis.LabelColor = Color.white;
            chart.YAxis.LabelSize = 16;
        }

        private void AddChartData() {
            LineDataSet set1 = new LineDataSet();
            set1.AddEntry(new LineEntry(0, 110));
            set1.AddEntry(new LineEntry(20, 50));
            set1.AddEntry(new LineEntry(40, 70));
            set1.AddEntry(new LineEntry(60, 130));
            set1.AddEntry(new LineEntry(80, 150));

            set1.LineColor = new Color32(54, 105, 126, 255);
            set1.FillColor = new Color32(54, 105, 126, 110);

            LineDataSet set2 = new LineDataSet();
            set2.AddEntry(new LineEntry(0, 80));
            set2.AddEntry(new LineEntry(20, 110));
            set2.AddEntry(new LineEntry(40, 95));
            set2.AddEntry(new LineEntry(60, 90));
            set2.AddEntry(new LineEntry(80, 120));

            set2.LineColor = new Color32(9, 107, 67, 255);
            set2.FillColor = new Color32(9, 107, 67, 110);

            chart.GetChartData().DataSets.Add(set1);
            chart.GetChartData().DataSets.Add(set2);

            chart.SetDirty();
        }
    }
}
