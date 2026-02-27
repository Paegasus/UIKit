using Xunit;

using UI.GFX.Geometry;

namespace UI.Tests;

public static class QuaternionTest
{
    private static double kEpsilon = 1e-7;

    private static void AssertQuaternion(Quaternion expected, Quaternion actual)
    {
        Assert.Equal(expected.X, actual.X, kEpsilon);
        Assert.Equal(expected.Y, actual.Y, kEpsilon);
        Assert.Equal(expected.Z, actual.Z, kEpsilon);
        Assert.Equal(expected.W, actual.W, kEpsilon);
    }

    private static void CompareQuaternions(Quaternion a, Quaternion b)
    {
        Assert.Equal(a.X, b.X, 1e-7);
        Assert.Equal(a.Y, b.Y, 1e-7);
        Assert.Equal(a.Z, b.Z, 1e-7);
        Assert.Equal(a.W, b.W, 1e-7);
    }

    [Fact]
    private static void TestDefaultConstruction()
    {
        CompareQuaternions(new Quaternion(0, 0, 0, 1), new Quaternion());
    }

    [Fact]
    private static void TestAxisAngleCommon()
    {
        double radians = 0.5;
        Quaternion q = new(new Vector3DF(1, 0, 0), radians);
        CompareQuaternions(new Quaternion(Math.Sin(radians / 2), 0, 0, Math.Cos(radians / 2)), q);
    }

    [Fact]
    private static void TestVectorToVectorRotation()
    {
        var q = new Quaternion(new Vector3DF(1.0f, 0.0f, 0.0f), new Vector3DF(0.0f, 1.0f, 0.0f));
        var r = new Quaternion(new Vector3DF(0.0f, 0.0f, 1.0f), MathF.PI / 2);

        Assert.Equal(r.X, q.X, 1e-7);
        Assert.Equal(r.Y, q.Y, 1e-7);
        Assert.Equal(r.Z, q.Z, 1e-7);
        Assert.Equal(r.W, q.W, 1e-7);
    }

    [Fact]
    private static void TestAxisAngleWithZeroLengthAxis()
    {
        var q = new Quaternion(new Vector3DF(0, 0, 0), 0.5);

        // If the axis is zero length, we should assume the default values.
        CompareQuaternions(q, new Quaternion());
    }

    [Fact]
    private static void TestAddition()
    {
        double[] values = [0, 1, 100];

        for (int i = 0; i < values.Length; ++i)
        {
            double t = values[i];
            Quaternion a = new(t, 2 * t, 3 * t, 4 * t);
            Quaternion b = new(5 * t, 4 * t, 3 * t, 2 * t);
            Quaternion sum = a + b;
            CompareQuaternions(new Quaternion(t, t, t, t) * 6, sum);
        }
    }

    [Fact]
    private static void TestMultiplication()
    {
        (Quaternion a, Quaternion b, Quaternion expected)[] cases =
        [
            (new Quaternion(1, 0, 0, 0), new Quaternion(1, 0, 0, 0), new Quaternion(0, 0, 0, -1)),
            (new Quaternion(0, 1, 0, 0), new Quaternion(0, 1, 0, 0), new Quaternion(0, 0, 0, -1)),
            (new Quaternion(0, 0, 1, 0), new Quaternion(0, 0, 1, 0), new Quaternion(0, 0, 0, -1)),
            (new Quaternion(0, 0, 0, 1), new Quaternion(0, 0, 0, 1), new Quaternion(0, 0, 0, 1)),
            (new Quaternion(1, 2, 3, 4), new Quaternion(5, 6, 7, 8), new Quaternion(24, 48, 48, -6)),
            (new Quaternion(5, 6, 7, 8), new Quaternion(1, 2, 3, 4), new Quaternion(32, 32, 56, -6))
        ];

        foreach(var (a, b, expected) in cases)
        {
            Quaternion product = a * b;
            CompareQuaternions(expected, product);
        }
    }

    [Fact]
    private static void TestScaling()
    {
        double[] values = [0, 10, 100];

        for (int i = 0; i < values.Length; ++i)
        {
            double s = values[i];
            Quaternion q = new(1, 2, 3, 4);
            Quaternion expected = new(s, 2 * s, 3 * s, 4 * s);
            CompareQuaternions(expected, q * s);
            CompareQuaternions(expected, s * q);
            if (s > 0)
                CompareQuaternions(expected, q / (1 / s));
        }
    }

    [Fact]
    private static void TestNormalization()
    {
        Quaternion q = new(1, -1, 1, -1);
        Assert.Equal(4, q.Length(), kEpsilon);

        q = q.Normalized();

        Assert.Equal(1, q.Length(), kEpsilon);
        Assert.Equal(0.5, q.X, kEpsilon);
        Assert.Equal(-0.5, q.Y, kEpsilon);
        Assert.Equal(0.5, q.Z, kEpsilon);
        Assert.Equal(-0.5, q.W, kEpsilon);
    }

    [Fact]
    private static void TestLerp()
    {
        for (int i = 1; i < 100; ++i)
        {
            Quaternion a = new(0, 0, 0, 0);
            Quaternion b = new(1, 2, 3, 4);
            float t = (float)i / 100.0f;
            Quaternion interpolated = a.Lerp(b, t);
            double s = 1.0 / Math.Sqrt(30.0);
            CompareQuaternions(new Quaternion(1, 2, 3, 4) * s, interpolated);
        }

        {
            Quaternion a = new(4, 3, 2, 1);
            Quaternion b = new(1, 2, 3, 4);
            CompareQuaternions(a.Normalized(), a.Lerp(b, 0));
            CompareQuaternions(b.Normalized(), a.Lerp(b, 1));
            CompareQuaternions(new Quaternion(1, 1, 1, 1).Normalized(), a.Lerp(b, 0.5));
        }
    }

    [Fact]
    private static void TestSlerp()
    {
        Vector3DF axis = new(1, 1, 1);
        double start_radians = -0.5;
        double stop_radians = 0.5;
        Quaternion start = new(axis, start_radians);
        Quaternion stop = new(axis, stop_radians);

        for (int i = 0; i < 100; ++i)
        {
            float t = (float)i / 100.0f;
            double radians = (1.0 - t) * start_radians + t * stop_radians;
            Quaternion expected = new(axis, radians);
            Quaternion interpolated = start.Slerp(stop, t);
            AssertQuaternion(expected, interpolated);
        }
    }

    [Fact]
    private static void TestSlerpOppositeAngles()
    {
        Vector3DF axis = new(1, 1, 1);
        double start_radians = -Math.PI / 2;
        double stop_radians = Math.PI / 2;
        Quaternion start = new(axis, start_radians);
        Quaternion stop = new(axis, stop_radians);

        // When quaternions are pointed in the fully opposite direction,
        // this is ambiguous, so we rotate as per https://www.w3.org/TR/css-transforms-1/
        Quaternion expected = new(axis, 0);

        Quaternion interpolated = start.Slerp(stop, 0.5f);
        AssertQuaternion(expected, interpolated);
    }

    [Fact]
    private static void TestSlerpRotateXRotateY()
    {
        var start = new Quaternion(new Vector3DF(1, 0, 0), Math.PI / 2);
        var stop = new Quaternion(new Vector3DF(0, 1, 0), Math.PI / 2);
        var interpolated = start.Slerp(stop, 0.5f);

        double expected_angle = Math.Acos(1.0 / 3.0);
        double xy = Math.Sin(0.5 * expected_angle) / Math.Sqrt(2);
        Quaternion expected = new(xy, xy, 0, Math.Cos(0.5 * expected_angle));
        AssertQuaternion(expected, interpolated);
    }

    [Fact]
    private static void TestSlerp360()
    {
        var start = new Quaternion(0, 0, 0, -1);  // 360 degree rotation.
        var stop = new Quaternion(new Vector3DF(0, 0, 1), Math.PI / 2);
        var interpolated = start.Slerp(stop, 0.5f);
        double expected_half_angle = Math.PI / 8;
        Quaternion expected = new(0, 0, Math.Sin(expected_half_angle),
                            Math.Cos(expected_half_angle));
        AssertQuaternion(expected, interpolated);
    }

    [Fact]
    private static void TestSlerpEquivalentQuaternions()
    {
        Quaternion start = new(new Vector3DF(1, 0, 0), Math.PI / 3);
        Quaternion stop = start.flip();
        Quaternion interpolated = start.Slerp(stop, 0.5f);
        AssertQuaternion(start, interpolated);
    }

    [Fact]
    private static void TestSlerpQuaternionWithInverse()
    {
        Quaternion start = new(new Vector3DF(1, 0, 0), Math.PI / 3);
        Quaternion stop = start.inverse();
        Quaternion interpolated = start.Slerp(stop, 0.5f);
        Quaternion expected = new(0, 0, 0, 1);
        AssertQuaternion(expected, interpolated);
    }

    [Fact]
    private static void TestSlerpObtuseAngle()
    {
        Quaternion start = new(new Vector3DF(1, 1, 0), Math.PI / 2);
        Quaternion stop = new(new Vector3DF(0, 1, -1), 3 * Math.PI / 2);
        Quaternion interpolated = start.Slerp(stop, 0.5f);
        double expected_half_angle = -Math.Atan(0.5);
        double xz = Math.Sin(expected_half_angle) / Math.Sqrt(2);
        Quaternion expected = new(xz, 0, xz, -Math.Cos(expected_half_angle));
        AssertQuaternion(expected, interpolated);
    }

    [Fact]
    private static void TestEquals()
    {
        Assert.True(new Quaternion() == new Quaternion());
        Assert.True(new Quaternion() == new Quaternion(0, 0, 0, 1));
        Assert.True(new Quaternion(1, 5.2, -8.5, 222.2) == new Quaternion(1, 5.2, -8.5, 222.2));
        Assert.False(new Quaternion() == new Quaternion(1, 0, 0, 0));
        Assert.False(new Quaternion() == new Quaternion(0, 1, 0, 0));
        Assert.False(new Quaternion() == new Quaternion(0, 0, 1, 0));
        Assert.False(new Quaternion() == new Quaternion(1, 0, 0, 1));
    }

    [Fact]
    private static void TestNotEquals()
    {
        Assert.False(new Quaternion() != new Quaternion());
        Assert.False(new Quaternion(1, 5.2, -8.5, 222.2) != new Quaternion(1, 5.2, -8.5, 222.2));
        Assert.True(new Quaternion() != new Quaternion(1, 0, 0, 0));
        Assert.True(new Quaternion() != new Quaternion(0, 1, 0, 0));
        Assert.True(new Quaternion() != new Quaternion(0, 0, 1, 0));
        Assert.True(new Quaternion() != new Quaternion(1, 0, 0, 1));
    }
}
