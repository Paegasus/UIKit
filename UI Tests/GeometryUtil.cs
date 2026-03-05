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

    public static void AssertFloatEqual(float a, float b) => Assert.True(FloatAlmostEqual(a, b));

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

    public static void AssertVector3DEqual(in Vector3D lhs, in Vector3D rhs)
    {
        Assert.True(FloatAlmostEqual((float)lhs.X, (float)rhs.X));
        Assert.True(FloatAlmostEqual((float)lhs.Y, (float)rhs.Y));
        Assert.True(FloatAlmostEqual((float)lhs.Z, (float)rhs.Z));
    }
    
    public static void AssertVector4DEqual(in Vector4D lhs, in Vector4D rhs)
    {
        Assert.True(FloatAlmostEqual((float)lhs.X, (float)rhs.X));
        Assert.True(FloatAlmostEqual((float)lhs.Y, (float)rhs.Y));
        Assert.True(FloatAlmostEqual((float)lhs.Z, (float)rhs.Z));
        Assert.True(FloatAlmostEqual((float)lhs.W, (float)rhs.W));
    }

    public static void AssertVector3DNear(in Vector3D lhs, in Vector3D rhs)
    {
        Assert.Equal((float)lhs.X, (float)rhs.X);
        Assert.Equal((float)lhs.Y, (float)rhs.Y);
        Assert.Equal((float)lhs.Z, (float)rhs.Z);
    }

    public static void AssertVector4DNear(in Vector4D lhs, in Vector4D rhs)
    {
        Assert.Equal((float)lhs.X, (float)rhs.X);
        Assert.Equal((float)lhs.Y, (float)rhs.Y);
        Assert.Equal((float)lhs.Z, (float)rhs.Z);
        Assert.Equal((float)lhs.W, (float)rhs.W);
    }

    public static void AssertDecomposedTransformEqual(in DecomposedTransform lhs, in DecomposedTransform rhs)
    {
        AssertVector3DEqual(lhs.Translate, rhs.Translate);
        AssertVector3DEqual(lhs.Scale, rhs.Scale);
        AssertVector3DEqual(lhs.Skew, rhs.Skew);
        AssertVector4DEqual(lhs.Perspective, rhs.Perspective);
	    AssertQuaternionEqual(lhs.Quaternion, rhs.Quaternion);
    }

    public static void AssertDecomposedTransformNear(in DecomposedTransform lhs, in DecomposedTransform rhs, float abs_error)
    {
        AssertVector3DNear(lhs.Translate, rhs.Translate);
        AssertVector3DNear(lhs.Scale, rhs.Scale);
        AssertVector3DNear(lhs.Skew, rhs.Skew);
        AssertVector4DNear(lhs.Perspective, rhs.Perspective);
        AssertQuaternionNear(lhs.Quaternion, rhs.Quaternion, abs_error);
    }
}
