using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace AwesomeCharts {
    public class ViewCreator : Object {

        private GameObject CreateBaseGameObject (string name, Transform parent, Vector2 pivot) {
            GameObject gameObject = new GameObject (name);
            gameObject.transform.SetParent (parent, false);
            gameObject.hideFlags = HideFlags.DontSave |
                HideFlags.HideInHierarchy |
                HideFlags.HideInInspector;
                
            RectTransform transfrom = gameObject.AddComponent<RectTransform> ();
            transfrom.anchorMin = new Vector2 (0, 0);
            transfrom.anchorMax = new Vector2 (0, 0);
            transfrom.pivot = pivot;
            transfrom.anchoredPosition = new Vector3 (0, 0, 0);

            return gameObject;
        }

        public ScrollRect InstantiateScroll (string name, Transform parent, Vector2 pivot) {
            GameObject scrollGameObject = CreateBaseGameObject (name, parent, pivot);
            scrollGameObject.AddComponent<CanvasRenderer> ();
            ScrollRect scrollRect = scrollGameObject.AddComponent<ScrollRect> ();

            scrollRect.viewport = InstantiateViewPort (scrollGameObject.transform);
            scrollRect.content = InstantiateContentView (scrollRect.viewport).GetComponent<RectTransform> ();
            scrollRect.vertical = false;

            return scrollRect;
        }

        private RectTransform InstantiateViewPort (Transform parent) {
            GameObject viewPort = CreateBaseGameObject ("ViewPort", parent, PivotValue.BOTTOM_LEFT);
            viewPort.AddComponent<Image> ();
            viewPort.AddComponent<Mask> ();
            viewPort.GetComponent<Mask> ().showMaskGraphic = false;

            return viewPort.GetComponent<RectTransform> ();
        }

        public GameObject InstantiateContentView (Transform parent) {
            GameObject content = CreateBaseGameObject ("Content", parent, PivotValue.BOTTOM_LEFT);
            content.AddComponent<CanvasRenderer> ();

            return content;
        }

        public GameObject InstantiateChartDataContainerView (Transform parent) {
            GameObject content = CreateBaseGameObject ("Content", parent, PivotValue.BOTTOM_LEFT);
            content.AddComponent<CanvasRenderer> ();

            return content;
        }

        public Text InstantiateBottomLabel (string name, Transform parent, Vector2 pivot) {
            Font arialFont = (Font) Resources.GetBuiltinResource (typeof (Font), "Arial.ttf");
            Text label = CreateBaseGameObject (name, parent, pivot).AddComponent<Text> ();
            label.font = arialFont;
            label.alignment = TextAnchor.MiddleCenter;

            return label.GetComponent<Text> ();
        }

        public XAxisRenderer InstantiateXAxisRenderer (string name, Transform parent, Vector2 pivot) {
            return CreateBaseGameObject (name, parent, pivot)
                .AddComponent<XAxisRenderer> ();
        }

        public XAxisBarChartRenderer InstantiateXAxisBarChartRenderer (string name, Transform parent, Vector2 pivot) {
            return CreateBaseGameObject (name, parent, pivot)
                .AddComponent<XAxisBarChartRenderer> ();
        }

        public YAxisRenderer InstantiateYAxisRenderer (string name, Transform parent, Vector2 pivot) {
            return CreateBaseGameObject (name, parent, pivot)
                .AddComponent<YAxisRenderer> ();
        }

        public UILineRenderer InstantiateLineRenderer (string name, Transform parent, Vector2 pivot) {
            UILineRenderer renderer = CreateBaseGameObject (name, parent, pivot)
                .AddComponent<UILineRenderer> ();
            renderer.raycastTarget = false;
            renderer.sprite = Resources.Load<Sprite> ("sprites/line_fill");

            return renderer;
        }

        public LineChartBackground InstantiateLineBackground (string name, Transform parent, Vector2 pivot) {
            LineChartBackground renderer = CreateBaseGameObject (name, parent, pivot)
                .AddComponent<LineChartBackground> ();
            renderer.raycastTarget = false;

            return renderer;
        }

        public LineEntryIdicator InstantiateCircleImage (string name, Transform parent) {
            GameObject gameObject = CreateBaseGameObject (name, parent, PivotValue.CENTER);
            LineEntryIdicator indicator = gameObject.AddComponent<LineEntryIdicator> ();
            indicator.button = gameObject.AddComponent<Button> ();
            indicator.image = gameObject.AddComponent<Image> ();
            indicator.image.sprite = Resources.Load<Sprite> ("sprites/line_chart_circle");
            indicator.button.targetGraphic = indicator.image;

            return indicator;
        }

        public Bar InstantiateBar (Transform parent, Bar barPrefab) {
            Bar bar = Instantiate (barPrefab ?? Resources.Load<Bar> ("prefabs/Bar"));
            bar.transform.SetParent (parent, false);

            return bar;
        }

        public ChartValuePopup InstantiateChartPopup (Transform parent, ChartValuePopup popupPrefab) {
            ChartValuePopup popup = Instantiate (popupPrefab ??
                Resources.Load<ChartValuePopup> ("prefabs/ChartValuePopup"));
            popup.GetComponent<RectTransform> ().pivot = PivotValue.BOTTOM_MIDDLE;
            popup.transform.SetParent (parent, false);

            return popup;
        }

        public LegendEntryView InstantiateLegendEntry (Transform parent, Vector2 pivot) {
            LegendEntryView entry = Instantiate (Resources.Load<LegendEntryView> ("prefabs/LegendEntryView"));
            RectTransform transform = entry.GetComponent<RectTransform> ();
            transform.pivot = pivot;
            transform.SetParent (parent, false);
            transform.anchorMin = new Vector2 (0, 0);
            transform.anchorMax = new Vector2 (0, 0);
            transform.anchoredPosition = Vector3.zero;

            return entry;
        }

        public PieChartEntryView InstantiatePieChartEntryView (string name, Transform parent, Vector2 pivot) {
            PieChartEntryView entryView = CreateBaseGameObject (name, parent, pivot)
                .AddComponent<PieChartEntryView> ();
            RectTransform transform = entryView.GetComponent<RectTransform> ();
            transform.anchorMin = new Vector2 (0.5f, 0.5f);
            transform.anchorMax = new Vector2 (0.5f, 0.5f);

            return entryView;
        }

        public GameObject InstantiateMaskablePieChartObject (string name, Transform parent, Vector2 pivot) {
            Image gameObject = CreateBaseGameObject (name, parent, pivot)
                .AddComponent<Image> ();
            gameObject.sprite = Resources.Load<Sprite> ("sprites/pie_chart_image");
            gameObject.material = Resources.Load<Material> ("materials/reversed_mask_material");
            RectTransform transform = gameObject.GetComponent<RectTransform> ();
            transform.anchorMin = new Vector2 (0.5f, 0.5f);
            transform.anchorMax = new Vector2 (0.5f, 0.5f);

            return gameObject.gameObject;
        }

        public PieChartValueIndicator InstantiatePieEntryValueIndicator (string name, Transform parent, Vector2 pivot) {
            PieChartValueIndicator indicatorView = CreateBaseGameObject (name, parent, pivot)
                .AddComponent<PieChartValueIndicator> ();
            indicatorView.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1f, 1f);
            indicatorView.GetComponent<RectTransform> ().anchorMin = new Vector2 (0.5f, 0.5f);
            indicatorView.GetComponent<RectTransform> ().anchorMax = new Vector2 (0.5f, 0.5f);

            return indicatorView;
        }

        public Text InstantiateText (string name, Transform parent, Vector2 pivot) {
            Text label = CreateBaseGameObject (name, parent, pivot)
                .AddComponent<Text> ();
            Font arialFont = (Font) Resources.GetBuiltinResource (typeof (Font), "Arial.ttf");
            label.font = arialFont;

            return label;
        }
    }
}