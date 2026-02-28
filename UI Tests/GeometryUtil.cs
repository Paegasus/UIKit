using Xunit;

using UI.GFX.Geometry;

namespace UI.Tests;

public static class GeometryUtil
{
    private static bool FloatAlmostEqual(float a, float b)
    {
        // Handle infinities and NaN explicitly, matching gtest's ULP-based AlmostEquals.
        if (float.IsNaN(a) || float.IsNaN(b)) return false;
        if (a == b) return true; // handles ∞ == ∞ and -∞ == -∞

        // ULP-based comparison matching gtest's FloatingPoint<float>::AlmostEquals,
        // which uses a threshold of 4 ULPs.
        int ulpA = BitConverter.SingleToInt32Bits(a);
        int ulpB = BitConverter.SingleToInt32Bits(b);
        if ((ulpA < 0) != (ulpB < 0)) return false; // different signs
        return Math.Abs(ulpA - ulpB) <= 4;
    }

    public static void AssertRectFEqual(in RectF lhs, in RectF rhs)
    {
        Assert.True(FloatAlmostEqual(lhs.X, rhs.X), $"X: {lhs.X} != {rhs.X}");
        Assert.True(FloatAlmostEqual(lhs.Y, rhs.Y), $"Y: {lhs.Y} != {rhs.Y}");
        Assert.True(FloatAlmostEqual(lhs.Width, rhs.Width), $"Width: {lhs.Width} != {rhs.Width}");
        Assert.True(FloatAlmostEqual(lhs.Height, rhs.Height), $"Height: {lhs.Height} != {rhs.Height}");
    }
}
