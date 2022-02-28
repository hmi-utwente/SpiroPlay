using UnityEngine;

namespace AwesomeCharts {
    [System.Serializable]
    public class AxisBase {

        [SerializeField]
        private int lineThickness = Defaults.AXIS_LINE_THICKNESS;
        [SerializeField]
        private Color lineColor = Defaults.AXIS_LINE_COLOR;
        [SerializeField]
        private float minAxisValue = Defaults.AXIS_MIN_VALUE;
        [SerializeField]
        private float maxAxisValue = Defaults.AXIS_MAX_VALUE;
        [SerializeField]
        private bool autoAxisMinValue = false;
        [SerializeField]
        private bool autoAxisMaxValue = false;
        [SerializeField]
        private int linesCount = Defaults.AXIS_LINES_COUNT;
        [SerializeField]
        private int labelSize = Defaults.AXIS_LABEL_SIZE;
        [SerializeField]
        private Color labelColor = Defaults.AXIS_LABEL_COLOR;
        [SerializeField]
        private float labelMargin = Defaults.AXIS_LABEL_MARGIN;
        [SerializeField]
        private Font labelFont;
        [SerializeField]
        private FontStyle labelFontStyle;
        [SerializeField]
        private bool dashedLine = true;
        [SerializeField]
        private bool shouldDrawLabels = true;
        [SerializeField]
        private bool shouldDrawLines = true;

        public int LineThickness {
            get { return lineThickness; }
            set { lineThickness = value; }
        }

        public Color LineColor {
            get { return lineColor; }
            set { lineColor = value; }
        }

        public float MinAxisValue {
            get { return minAxisValue; }
            set { minAxisValue = value; }
        }

        public float MaxAxisValue {
            get { return maxAxisValue; }
            set { maxAxisValue = value; }
        }

        public bool AutoAxisMaxValue {
            get { return autoAxisMaxValue; }
            set { autoAxisMaxValue = value; }
        }

        public bool AutoAxisMinValue {
            get { return autoAxisMinValue; }
            set { autoAxisMinValue = value; }
        }

        public int LinesCount {
            get { return linesCount; }
            set { linesCount = value; }
        }

        public int LabelSize {
            get { return labelSize; }
            set { labelSize = value; }
        }

        public Color LabelColor {
            get { return labelColor; }
            set { labelColor = value; }
        }

        public float LabelMargin {
            get { return labelMargin; }
            set { labelMargin = value; }
        }

        public Font LabelFont {
            get { return labelFont; }
            set { labelFont = value; }
        }

        public FontStyle LabelFontStyle {
            get { return labelFontStyle; }
            set { labelFontStyle = value; }
        }

        public bool DashedLine {
            get { return dashedLine; }
            set { dashedLine = value; }
        }

        public bool ShouldDrawLabels {
            get { return shouldDrawLabels; }
            set { shouldDrawLabels = value; }
        }

        public bool ShouldDrawLines {
            get { return shouldDrawLines; }
            set { shouldDrawLines = value; }
        }
    }
}