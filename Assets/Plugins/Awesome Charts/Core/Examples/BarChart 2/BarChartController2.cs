using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwesomeCharts {
    public class BarChartController2 : MonoBehaviour {

        public BarChart chart;

        private void Start() {
            ConfigChart();
            AddChartData();
        }

        private void ConfigChart() {
            chart.Config.BarWidth = 45;
            chart.Config.BarSpacing = 20;
            chart.Config.InnerBarSpacing = 8;
            chart.Config.SizingMethod = BarSizingMethod.SIZE_TO_FIT;

            chart.XAxis.DashedLine = true;
            chart.XAxis.LineThickness = 1;
            chart.XAxis.LabelColor = Color.white;
            chart.XAxis.LabelSize = 16;

            chart.YAxis.LabelSize = 16;
        }

        private void AddChartData() {
            BarDataSet set1 = new BarDataSet("Data Set 1");
            set1.AddEntry(new BarEntry(0, 100));
            set1.AddEntry(new BarEntry(1, 50));
            set1.AddEntry(new BarEntry(2, 70));
            set1.AddEntry(new BarEntry(3, 130));
            set1.AddEntry(new BarEntry(4, 150));

            set1.BarColors.Add(new Color32(125, 163, 161, 255));

            BarDataSet set2 = new BarDataSet("Data Set 2");
            set2.AddEntry(new BarEntry(0, 80));
            set2.AddEntry(new BarEntry(1, 110));
            set2.AddEntry(new BarEntry(2, 75));
            set2.AddEntry(new BarEntry(3, 90));
            set2.AddEntry(new BarEntry(4, 130));

            set2.BarColors.Add(new Color32(52, 103, 93, 255));

            List<string> labels = new List<string>();
            labels.Add("Mon");
            labels.Add("Tue");
            labels.Add("Wed");
            labels.Add("Thu");
            labels.Add("Fri");

            chart.GetChartData().CustomLabels = labels;
            chart.GetChartData().DataSets.Add(set1);
            chart.GetChartData().DataSets.Add(set2);

            chart.SetDirty();
        }
    }
}
