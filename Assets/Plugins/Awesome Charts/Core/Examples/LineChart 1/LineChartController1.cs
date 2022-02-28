using UnityEngine;
using UnityEngine.UI;

namespace AwesomeCharts {
    public class LineChartController1 : MonoBehaviour {

        public LineChart chart;
        public InputField xInputField;
        public InputField yInputField;

        public void OnAddEntryClick() {
            int x = IntFromText(xInputField.text);
            int y = IntFromText(yInputField.text);
            chart.GetChartData().DataSets[0].AddEntry(new LineEntry(x, y));
            chart.SetDirty();
        }

        public void OnRemoveEntryClick() {
            LineDataSet dataSet = chart.GetChartData().DataSets[0];
            if (dataSet.Entries.Count > 0) {
                dataSet.RemoveEntry(dataSet.Entries.Count-1);
                chart.SetDirty();
            }
        }

        private int IntFromText(string text) {
            int result;
            if(!int.TryParse(text, out result)) {
                result = 0;
            }
            return result;
        }
    }
}
