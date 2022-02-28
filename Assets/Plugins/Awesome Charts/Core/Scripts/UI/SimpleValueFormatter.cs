using System;

namespace AwesomeCharts {

    public class SimpleValueFormatter : IValueFormatter {

        public string FormatAxisValue(float value, float min, float max) {
            int decimalPlaces = CalculateDecimalPlaces(Math.Abs(max-min));
            return value.ToString("F"+decimalPlaces);
        }

        private int CalculateDecimalPlaces(float minMaxDelta) {
            if (minMaxDelta >= 100f) {
                return 0;
            } else if (minMaxDelta >= 10f) {
                return 1;
            } else if (minMaxDelta >= 0.1f) {
                return 2;
            } else if (minMaxDelta >= 0.001) {
                return 4;
            } else {
                return 7;
            }
        }
    }
}
