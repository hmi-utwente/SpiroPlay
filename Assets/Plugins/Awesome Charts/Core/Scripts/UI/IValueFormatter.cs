using System;

namespace AwesomeCharts {

    public interface IValueFormatter {

        string FormatAxisValue(float value, float min, float max);
    }
}
