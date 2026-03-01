using Xunit;

using UI.GFX.Geometry;

namespace UI.Tests;

public static class GeometryUtil
{
    public static bool FloatAlmostEqual(float a, float b)
    {
        // Handle infinities and NaN explicitly, matching gtest's ULP-based AlmostEquals.
        if (float.IsNaN(a) || float.IsNaN(b)) return false;

        int bitsA = BitConverter.SingleToInt32Bits(a);
        int bitsB = BitConverter.SingleToInt32Bits(b);

        // Convert sign-and-magnitude to biased representation, matching gtest's DistanceBetweenSignAndMagnitudeNumbers.
        static uint Biased(int bits) => (bits < 0)
                                      ? (uint)(~bits + 1)
                                      : (uint)bits | 0x80000000u;

        uint distance = Biased(bitsA) >= Biased(bitsB)
            ? Biased(bitsA) - Biased(bitsB)
            : Biased(bitsB) - Biased(bitsA);

        return distance <= 4;
    }

    public static void AssertRectFEqual(in RectF lhs, in RectF rhs)
    {
        Assert.True(FloatAlmostEqual(lhs.X, rhs.X), $"X: {lhs.X} != {rhs.X}");
        Assert.True(FloatAlmostEqual(lhs.Y, rhs.Y), $"Y: {lhs.Y} != {rhs.Y}");
        Assert.True(FloatAlmostEqual(lhs.Width, rhs.Width), $"Width: {lhs.Width} != {rhs.Width}");
        Assert.True(FloatAlmostEqual(lhs.Height, rhs.Height), $"Height: {lhs.Height} != {rhs.Height}");
    }
}
