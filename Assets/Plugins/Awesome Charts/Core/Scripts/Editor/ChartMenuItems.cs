using UnityEngine;
using UnityEditor;

namespace AwesomeCharts {

    public class ChartMenuItems {

        [MenuItem("GameObject/UI/BarChart")]
        private static void AddBarChartOption() {
            GameObject gameObject = new GameObject("BarChart");
            gameObject.AddComponent<BarChart>();
            if (Selection.transforms.Length > 0) {
                gameObject.GetComponent<RectTransform>().SetParent(Selection.transforms[0], false);
            }
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 200f);
        }

        [MenuItem("GameObject/UI/LineChart")]
        private static void AddLineChartOption() {
            GameObject gameObject = new GameObject("LineChart");
            gameObject.AddComponent<LineChart>();
            if (Selection.transforms.Length > 0) {
                gameObject.GetComponent<RectTransform>().SetParent(Selection.transforms[0], false);
            }
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 200f);
        }

        [MenuItem("GameObject/UI/PieChart")]
        private static void AddPieChartOption() {
            GameObject gameObject = new GameObject("PieChart");
            gameObject.AddComponent<PieChart>();
            if (Selection.transforms.Length > 0) {
                gameObject.GetComponent<RectTransform>().SetParent(Selection.transforms[0], false);
            }
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(200f, 200f);
        }

        [MenuItem("GameObject/UI/LegendView")]
        private static void AddLegendViewOption() {
            GameObject gameObject = new GameObject("LegendView");
            gameObject.AddComponent<LegendView>();
            if (Selection.transforms.Length > 0) {
                gameObject.GetComponent<RectTransform>().SetParent(Selection.transforms[0], false);
            }
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
        }
    }
}
