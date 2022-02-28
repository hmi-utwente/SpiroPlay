using System;

namespace AwesomeCharts {

    public class XAxisBarChartFormatter : IValueFormatter {

        public string FormatAxisValue(float value, float min, float max) {
            return value.ToString();
        }
    }
}
