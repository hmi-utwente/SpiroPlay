using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AwesomeCharts {

    [ExecuteInEditMode]
    public class BarChart : AxisBaseChart<BarData> {

        [SerializeField]
        private BarChartConfig config;

        [SerializeField]
        internal BarData data;

        private BarCharPositioner positioner = new BarCharPositioner ();
        private ChartValuePopup currentValuePopup = null;
        private BarEntry currentValuePopupEntry = null;

        private List<Bar> barInstances = new List<Bar> ();

        public BarChartConfig Config {
            get { return config; }
            set {
                config = value;
                config.configChangeListener = OnConfigChanged;
            }
        }

        public override BarData GetChartData () {
            return data;
        }

        void Reset () {
            data = new BarData (new BarDataSet ());
            Config = new BarChartConfig ();
        }

        protected override void Awake () {
            base.Awake ();

            if (data == null) {
                data = new BarData (new BarDataSet ());
            }
            if (config == null) {
                Config = new BarChartConfig ();
            }

            XAxis.AutoAxisMinValue = true;
            XAxis.AutoAxisMaxValue = true;
        }

        private void OnConfigChanged () {
            SetDirty ();
        }

        protected override void OnInstantiateViews () {
            base.OnInstantiateViews ();
            chartDataContainerView.AddComponent<RectMask2D> ();
            UpdateBarChartPositioner ();
        }

        protected override void OnUpdateAxis () {
            base.OnUpdateAxis ();
            UpdateBarChartPositioner ();
        }

        private void UpdateBarChartPositioner () {
            positioner.data = data;
            positioner.barChartConfig = Config;
            positioner.containerSize = GetSize ();
            positioner.axisBounds = GetAxisBounds ();
            positioner.RecalculatePositioner ();
        }

        protected override List<LegendEntry> CreateLegendViewEntries () {
            List<LegendEntry> result = new List<LegendEntry> ();
            foreach (BarDataSet dataSet in data.DataSets) {
                result.Add (new LegendEntry (dataSet.Title,
                    dataSet.GetColorForIndex (0)));
            }

            return result;
        }

        protected override void OnDrawChartContent () {
            base.OnDrawChartContent ();

            UpdateBarInstances (positioner.GetAllVisibleEntriesCount ());
            if (GetChartData ().HasAnyData ()) {
                ShowBars ();
            }
        }

        override protected XAxisRenderer InstantiateXAxisRenderer () {
            XAxisBarChartRenderer xRenderer = viewCreator.InstantiateXAxisBarChartRenderer ("XAxis", contentView.transform, PivotValue.BOTTOM_LEFT);
            xRenderer.barChartPositioner = positioner;

            return xRenderer;
        }

        private void UpdateBarInstances (int requiredCount) {
            int currentBarsCount = barInstances.Count;

            // Add missing bars
            int missingBarsCount = requiredCount - currentBarsCount;
            while (missingBarsCount > 0) {
                Bar barInstance = viewCreator.InstantiateBar (chartDataContainerView.transform, Config.BarPrefab);
                barInstances.Add (barInstance);
                missingBarsCount--;
            }

            // Remove redundant bars
            int redundantBarsCount = currentBarsCount - requiredCount;
            while (redundantBarsCount > 0) {
                Bar target = barInstances[barInstances.Count - 1];
                DestroyDelayed (target.gameObject);
                barInstances.Remove (target);
                redundantBarsCount--;
            }
        }

        private void ShowBars () {
            int nextBarInstanceIndex = 0;

            for (int i = 0; i < data.DataSets.Count; i++) {
                nextBarInstanceIndex = UpdatedBars (i, nextBarInstanceIndex);
            }
        }

        private int UpdatedBars (int dataSetIndex, int nextBarInstanceIndex) {
            List<BarEntry> barEntries = positioner.GetVisibleEntries (dataSetIndex);
            for (int i = 0; i < barEntries.Count; i++) {
                UpdateBarWithEntry (barInstances[nextBarInstanceIndex],
                    barEntries[i],
                    data.DataSets[dataSetIndex].GetColorForIndex (i),
                    dataSetIndex);
                nextBarInstanceIndex++;
            }

            return nextBarInstanceIndex;
        }

        private Bar UpdateBarWithEntry (Bar barInstance, BarEntry entry, Color color, int dataSetIndex) {
            barInstance.transform.localPosition = positioner.GetBarPosition ((int) entry.Position, dataSetIndex);
            barInstance.GetComponent<RectTransform> ().sizeDelta = positioner.GetBarSize (entry.Value);
            barInstance.SetColor (color);
            barInstance.button.onClick.RemoveAllListeners ();
            barInstance.button.onClick.AddListener (delegate { OnBarClick (entry, dataSetIndex); });

            return barInstance;
        }

        private void OnBarClick (BarEntry entry, int dataSetIndex) {
            if (Config.BarChartClickAction != null) {
                Config.BarChartClickAction.Invoke (entry, dataSetIndex);
            }
            ShowHideValuePopup (entry, dataSetIndex);
        }

        private void ShowHideValuePopup (BarEntry entry, int dataSetIndex) {
            if (currentValuePopup == null) {
                currentValuePopup = viewCreator.InstantiateChartPopup (contentView.transform, Config.PopupPrefab);
            }

            if (entry != currentValuePopupEntry) {
                UpdateValuePopup (entry, dataSetIndex);
                currentValuePopupEntry = entry;
            } else {
                currentValuePopup.gameObject.SetActive (false);
                currentValuePopupEntry = null;
            }
        }

        private void UpdateValuePopup (BarEntry entry, int dataSetIndex) {
            currentValuePopup.transform.localPosition = positioner.GetValuePopupPosition (entry, dataSetIndex);
            currentValuePopup.text.text = "" + entry.Value;
            currentValuePopup.gameObject.SetActive (true);
        }
    }
}