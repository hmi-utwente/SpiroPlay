namespace Spirometry.Statics
{
    public static class ExtensionMethods
    {
        //remapping of float values
        public static float Remap(this float value, float fromMinimum, float fromMaximum, float toMinimum, float toMaximum)
        {
            return (value - fromMinimum) / (fromMaximum - fromMinimum) * (toMaximum - toMinimum) + toMinimum;
        }

    }
}
