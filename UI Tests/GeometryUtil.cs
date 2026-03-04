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

    public static void AssertPointNear(in Point lhs, in Point rhs, float tolerance)
    {
        Assert.Equal(lhs.X, rhs.X, tolerance);
        Assert.Equal(lhs.Y, rhs.Y, tolerance);
    }

    public static void AssertPointFNear(in PointF lhs, in PointF rhs, float tolerance)
    {
        Assert.Equal(lhs.X, rhs.X, tolerance);
        Assert.Equal(lhs.Y, rhs.Y, tolerance);
    }

    public static void AssertPoint3FNear(in Point3F lhs, in Point3F rhs, float tolerance)
    {
        Assert.Equal(lhs.X, rhs.X, tolerance);
        Assert.Equal(lhs.Y, rhs.Y, tolerance);
        Assert.Equal(lhs.Z, rhs.Z, tolerance);
    }

    public static void AssertPointEqual(in Point lhs, in Point rhs)
    {
        Assert.True(FloatAlmostEqual(lhs.X, rhs.X));
        Assert.True(FloatAlmostEqual(lhs.Y, rhs.Y));
    }

    public static void AssertPointFEqual(in PointF lhs, in PointF rhs)
    {
        Assert.True(FloatAlmostEqual(lhs.X, rhs.X));
        Assert.True(FloatAlmostEqual(lhs.Y, rhs.Y));
    }

    public static void AssertPoint3FEqual(in Point3F lhs, in Point3F rhs)
    {
        Assert.True(FloatAlmostEqual(lhs.X, rhs.X));
        Assert.True(FloatAlmostEqual(lhs.Y, rhs.Y));
        Assert.True(FloatAlmostEqual(lhs.Z, rhs.Z));
    }

    public static void AssertQuaternionEqual(in Quaternion lhs, in Quaternion rhs)
    {
        Assert.True(FloatAlmostEqual((float)lhs.X, (float)rhs.X));
        Assert.True(FloatAlmostEqual((float)lhs.Y, (float)rhs.Y));
        Assert.True(FloatAlmostEqual((float)lhs.Z, (float)rhs.Z));
        Assert.True(FloatAlmostEqual((float)lhs.W, (float)rhs.W));
    }

    public static void AssertQuaternionNear(in Quaternion lhs, in Quaternion rhs, float abs_error)
    {
        Assert.Equal(lhs.X, rhs.X, abs_error);
        Assert.Equal(lhs.Y, rhs.Y, abs_error);
        Assert.Equal(lhs.Z, rhs.Z, abs_error);
        Assert.Equal(lhs.W, rhs.W, abs_error);
    }

    public static void AssertTransformEqual(in Transform lhs, in Transform rhs)
    {
        for (int row = 0; row < 4; ++row)
        {
            for (int col = 0; col < 4; ++col)
            {
                Assert.True(FloatAlmostEqual((float)lhs.rc(row, col), (float)rhs.rc(row, col)));
            }
        }
    }

    public static void AssertTransformNear(in Transform lhs, in Transform rhs, float abs_error)
    {
        for (int row = 0; row < 4; ++row)
        {
            for (int col = 0; col < 4; ++col)
            {
                Assert.Equal(lhs.rc(row, col), rhs.rc(row, col), abs_error);
            }
        }
    }

    public static void AssertDecomposedTransformEqual(in DecomposedTransform lhs, in DecomposedTransform rhs)
    {
        Assert.True(FloatAlmostEqual((float)lhs.Translate.X, (float)rhs.Translate.X), "Translate.X");
        Assert.True(FloatAlmostEqual((float)lhs.Translate.Y, (float)rhs.Translate.Y), "Translate.Y");
        Assert.True(FloatAlmostEqual((float)lhs.Translate.Z, (float)rhs.Translate.Z), "Translate.Z");

        Assert.True(FloatAlmostEqual((float)lhs.Scale.X, (float)rhs.Scale.X), "Scale.X");
        Assert.True(FloatAlmostEqual((float)lhs.Scale.Y, (float)rhs.Scale.Y), "Scale.Y");
        Assert.True(FloatAlmostEqual((float)lhs.Scale.Z, (float)rhs.Scale.Z), "Scale.Z");

        Assert.True(FloatAlmostEqual((float)lhs.Skew.X, (float)rhs.Skew.X), "Skew.X");
        Assert.True(FloatAlmostEqual((float)lhs.Skew.Y, (float)rhs.Skew.Y), "Skew.Y");
        Assert.True(FloatAlmostEqual((float)lhs.Skew.Z, (float)rhs.Skew.Z), "Skew.Z");

        Assert.True(FloatAlmostEqual((float)lhs.Perspective.X, (float)rhs.Perspective.X), "Perspective.X");
        Assert.True(FloatAlmostEqual((float)lhs.Perspective.Y, (float)rhs.Perspective.Y), "Perspective.Y");
        Assert.True(FloatAlmostEqual((float)lhs.Perspective.Z, (float)rhs.Perspective.Z), "Perspective.Z");
        Assert.True(FloatAlmostEqual((float)lhs.Perspective.W, (float)rhs.Perspective.W), "Perspective.W");

        AssertQuaternionEqual(lhs.Quaternion, rhs.Quaternion);
    }
}
