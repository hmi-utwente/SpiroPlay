using System;
using System.Collections.Generic;
using UnityEngine;

namespace AwesomeCharts {

    [Serializable]
    public abstract class AxisChartData : ChartData {

        [SerializeField]
        private List<string> customLabels;

        public List<string> CustomLabels {
            get { return customLabels; }
            set { customLabels = value; }
        }

        public abstract float GetMinPosition();

        public abstract float GetMaxPosition();

        public abstract float GetMinValue();

        public abstract float GetMaxValue();
    }
}