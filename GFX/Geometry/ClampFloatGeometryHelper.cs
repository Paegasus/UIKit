namespace UI.GFX.Geometry;

public static class ClampFloatGeometryHelper
{
    public static float Max => float.MaxValue / 1e6f;
    public static float Min => float.MinValue / 1e6f;

    // Clamps |value| (float, double) within the range of
    // [float.MinValue / 1e6f, float.MaxValue / 1e6f].
    // Returns 0 for NaN. This avoids NaN and infinity values immediately, and
    // reduce the chance of producing NaN and infinity values for future unclamped
    // operations like offsetting and scaling by devices / page scale factor.

    public static float ClampFloatGeometry(float value) 
    { 
        if (float.IsNaN(value)) return 0f;
        if (value > Max) return Max;
        if (value < Min) return Min;
        return value;
    }

    public static float ClampFloatGeometry(double value) 
    {
        if (double.IsNaN(value)) return 0f;
        if (value > Max) return Max;
        if (value < Min) return Min;
        return (float)value;
    }
}
