using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace AwesomeCharts {

    [ExecuteInEditMode]
    [RequireComponent (typeof (CanvasRenderer))]
    public class PieChartValueIndicator : MonoBehaviour {

        [SerializeField]
        private String label;
        [SerializeField]
        private int fontSize = 14;
        [SerializeField]
        private Color indicatorColor = Color.white;
        [SerializeField]
        private List<Vector2> linePoints;
        [SerializeField]
        private Boolean reversedLabel = false;

        public String Label {
            get { return label; }
            set {
                label = value;
                SetDirty ();
            }
        }

        public int FontSize {
            get { return fontSize; }
            set {
                fontSize = value;
                SetDirty ();
            }
        }

        public Color IndicatorColor {
            get { return indicatorColor; }
            set {
                indicatorColor = value;
                SetDirty ();
            }
        }

        public List<Vector2> LinePoints {
            get { return linePoints; }
            set {
                linePoints = value;
                SetDirty ();
            }
        }

        public Boolean ReversedLabel {
            get { return reversedLabel; }
            set {
                reversedLabel = value;
                SetDirty ();
            }
        }

        private Text labelText;
        private UILineRenderer lineRenderer;

        private Boolean isDirty = true;
        private ViewCreator viewCreator = new ViewCreator ();

        public void SetDirty () {
            isDirty = true;
        }

        void Awake () {
            if (LinePoints == null) {
                linePoints = new List<Vector2> ();
            }
        }

        void Start () {
            ClearEditModeObjects ();
            InstantiateViews ();
        }

        private void ClearEditModeObjects () {
            int children = transform.childCount;
            for (int i = 0; i < children; i++) {
                DestroyImmediate (transform.GetChild (0).gameObject);
            }
        }

        private void InstantiateViews () {
            labelText = viewCreator.InstantiateText ("label", transform, PivotValue.MIDDLE_LEFT);
            labelText.GetComponent<RectTransform> ().anchorMin = new Vector2 (0.5f, 0.5f);
            labelText.GetComponent<RectTransform> ().anchorMax = new Vector2 (0.5f, 0.5f);
            labelText.GetComponent<RectTransform> ().sizeDelta = new Vector2 (300f, 40f);

            lineRenderer = viewCreator.InstantiateLineRenderer ("line", transform, PivotValue.CENTER);
            lineRenderer.GetComponent<RectTransform> ().anchorMin = new Vector2 (0.5f, 0.5f);
            lineRenderer.GetComponent<RectTransform> ().anchorMax = new Vector2 (0.5f, 0.5f);
            lineRenderer.lineThickness = 2.0f;
        }

        private void OnValidate () {
            SetDirty ();
        }

        void Update () {
            if (isDirty) {
                UpdateView ();
                isDirty = false;
            }
        }

        private void UpdateView () {
            if (labelText == null || lineRenderer == null) {
                return;
            }

            labelText.text = Label;
            labelText.color = IndicatorColor;
            labelText.fontSize = FontSize;

            lineRenderer.Points = LinePoints.ToArray ();
            lineRenderer.color = IndicatorColor;

            if (ReversedLabel) {
                labelText.alignment = TextAnchor.MiddleRight;
                labelText.GetComponent<RectTransform> ().pivot = PivotValue.MIDDLE_RIGHT;
            } else {
                labelText.alignment = TextAnchor.MiddleLeft;
                labelText.GetComponent<RectTransform> ().pivot = PivotValue.MIDDLE_LEFT;
            }
        }
    }
}