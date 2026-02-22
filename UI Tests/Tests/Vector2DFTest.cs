using Xunit;

using UI.GFX.Geometry;

namespace UI.Tests;

public static class Vector2DFTest
{
    private static bool FloatEqual(float a, float b) => MathF.Abs(a - b) <= 1e-6f;

    private static bool FloatNear(float val1, float val2, float abs_error) => MathF.Abs(val1 - val2) <= abs_error;

    [Fact]
    private static void TestVector2DToVector2DF()
    {
        Vector2D i = new(3, 4);
        Vector2DF f = i;
        Assert.Equal(i, f);
    }

    [Fact]
    private static void TestIsZero()
    {
        Assert.True(new Vector2DF().IsZero());
        Assert.True(new Vector2DF(0, 0).IsZero());
        Assert.False(new Vector2DF(0.1f, 0).IsZero());
        Assert.False(new Vector2DF(0, -0.1f).IsZero());
        Assert.False(new Vector2DF(0.1f, -0.1f).IsZero());
    }

    [Fact]
    private static void TestAdd()
    {
        Vector2DF f1 = new(3.1f, 5.1f);
        Vector2DF f2 = new(4.3f, -1.3f);
        Assert.Equal(new Vector2DF(3.1f, 5.1f), f1 + new Vector2DF());
        Assert.Equal(new Vector2DF(3.1f + 4.3f, 5.1f - 1.3f), f1 + f2);
        Assert.Equal(new Vector2DF(3.1f - 4.3f, 5.1f + 1.3f), f1 - f2);
    }

    [Fact]
    private static void TestNegative()
    {
        Assert.Equal(new Vector2DF(), -new Vector2DF());
        Assert.Equal(new Vector2DF(-0.3f, -0.3f), -new Vector2DF(0.3f, 0.3f));
        Assert.Equal(new Vector2DF(0.3f, 0.3f), -new Vector2DF(-0.3f, -0.3f));
        Assert.Equal(new Vector2DF(-0.3f, 0.3f), -new Vector2DF(0.3f, -0.3f));
        Assert.Equal(new Vector2DF(0.3f, -0.3f), -new Vector2DF(-0.3f, 0.3f));
    }

    [Fact]
    private static void TestScale()
    {
        (float, float, float, float)[] double_values =
        [
            (4.5f, 1.2f, 3.3f, 5.6f),  (4.5f, -1.2f, 3.3f, 5.6f),
            (4.5f, 1.2f, 3.3f, -5.6f), (4.5f, 1.2f, -3.3f, -5.6f),
            (-4.5f, 1.2f, 3.3f, 5.6f), (-4.5f, 1.2f, 0, 5.6f),
            (-4.5f, 1.2f, 3.3f, 0),    (4.5f, 0, 3.3f, 5.6f),
            (0, 1.2f, 3.3f, 5.6f)
        ];

        foreach (var (X, Y, Z, W) in double_values)
        {
            Vector2DF v = new(X, Y);
            v.Scale(Z, W);
            Assert.Equal(v.X, X * Z);
            Assert.Equal(v.Y, Y * W);

            Vector2DF v2 = Vector2DF.ScaleVector2D(new Vector2DF(X, Y), Z, W);
            Assert.Equal(X * Z, v2.X);
            Assert.Equal(Y * W, v2.Y);
        }

        (float, float, float)[] single_values =
        [
            (4.5f, 1.2f, 3.3f),  (4.5f, -1.2f, 3.3f), (4.5f, 1.2f, 3.3f),
            (4.5f, 1.2f, -3.3f), (-4.5f, 1.2f, 3.3f), (-4.5f, 1.2f, 0),
            (-4.5f, 1.2f, 3.3f), (4.5f, 0, 3.3f),     (0, 1.2f, 3.3f)
        ];

        foreach (var (X, Y, Z) in single_values)
        {
            Vector2DF v = new(X, Y);
            v.Scale(Z);
            Assert.Equal(v.X, X * Z);
            Assert.Equal(v.Y, Y * Z);

            Vector2DF v2 = Vector2DF.ScaleVector2D(new Vector2DF(X, Y), Z);
            Assert.Equal(X * Z, v2.X);
            Assert.Equal(Y * Z, v2.Y);
        }
    }

    [Fact]
    private static void SetToMinMax()
    {
        Vector2DF a;

        a = new Vector2DF(3.5f, 5.5f);
        Assert.Equal(new Vector2DF(3.5f, 5.5f), a);
        a.SetToMax(new Vector2DF(2.5f, 4.5f));
        Assert.Equal(new Vector2DF(3.5f, 5.5f), a);
        a.SetToMax(new Vector2DF(3.5f, 5.5f));
        Assert.Equal(new Vector2DF(3.5f, 5.5f), a);
        a.SetToMax(new Vector2DF(4.5f, 2.5f));
        Assert.Equal(new Vector2DF(4.5f, 5.5f), a);
        a.SetToMax(new Vector2DF(8.5f, 10.5f));
        Assert.Equal(new Vector2DF(8.5f, 10.5f), a);

        a.SetToMin(new Vector2DF(9.5f, 11.5f));
        Assert.Equal(new Vector2DF(8.5f, 10.5f), a);
        a.SetToMin(new Vector2DF(8.5f, 10.5f));
        Assert.Equal(new Vector2DF(8.5f, 10.5f), a);
        a.SetToMin(new Vector2DF(11.5f, 9.5f));
        Assert.Equal(new Vector2DF(8.5f, 9.5f), a);
        a.SetToMin(new Vector2DF(7.5f, 11.5f));
        Assert.Equal(new Vector2DF(7.5f, 9.5f), a);
        a.SetToMin(new Vector2DF(3.5f, 5.5f));
        Assert.Equal(new Vector2DF(3.5f, 5.5f), a);
    }

    [Fact]
    private static void TestLength()
    {
        float tolerance = 1e-6f;

        Assert.Equal(0.0f, new Vector2DF(0, 0).Length());
        Assert.Equal(1.0f, new Vector2DF(1, 0).Length());
        Assert.Equal(1.414214f, new Vector2DF(1, 1).Length(), tolerance);
        Assert.Equal(2.236068f, new Vector2DF(-1, -2).Length());

        // The Pythagorean triples 3-4-5 and 5-12-13.
        Assert.Equal(5.0f, new Vector2DF(3.0f, 4.0f).Length());
        Assert.Equal(13.0f, new Vector2DF(5.0f, 12.0f).Length());

        // Very small numbers.
        Assert.Equal(0.7071068e-20f, new Vector2DF(0.5e-20f, 0.5e-20f).Length());

        // Very large numbers.
        Assert.Equal(0.7071068e20f, new Vector2DF(0.5e20f, 0.5e20f).Length());
        Assert.Equal(float.MaxValue, new Vector2DF(float.MaxValue, 0).Length());
        // The original C++ code checks if Length() returns float.MaxValue, but that's impossible,
        // since float.Hypot(float.MaxValue, float.MaxValue) returns positive infinity
        //Assert.True(FloatEq(float.MaxValue, new Vector2DF(float.MaxValue, float.MaxValue).Length()));
        Assert.Equal(float.PositiveInfinity, new Vector2DF(float.MaxValue, float.MaxValue).Length());
    }

    [Fact]
    private static void TestSlopeAngleRadians()
    {
        // The function is required to be very accurate, so we use a smaller tolerance than EXPECT_FLOAT_EQ().
        float kTolerance = 1e-7f;
        float kPi = 3.1415927f;
        Assert.True(FloatNear(0, new Vector2DF(0, 0).SlopeAngleRadians(), kTolerance));
        Assert.True(FloatNear(0, new Vector2DF(1, 0).SlopeAngleRadians(), kTolerance));
        Assert.True(FloatNear(kPi / 4, new Vector2DF(1, 1).SlopeAngleRadians(), kTolerance));
        Assert.True(FloatNear(kPi / 2, new Vector2DF(0, 1).SlopeAngleRadians(), kTolerance));
        Assert.True(FloatNear(kPi, new Vector2DF(-50, 0).SlopeAngleRadians(), kTolerance));
        Assert.True(FloatNear(-kPi * 3 / 4, new Vector2DF(-50, -50).SlopeAngleRadians(), kTolerance));
        Assert.True(FloatNear(-kPi / 4, new Vector2DF(1, -1).SlopeAngleRadians(), kTolerance));
    }

    [Fact]
    private static void TestTranspose()
    {
        Vector2DF v = new(-1.5f, 2.5f);
        Assert.Equal(new Vector2DF(2.5f, -1.5f), Vector2DF.TransposeVector2D(v));
        v.Transpose();
        Assert.Equal(new Vector2DF(2.5f, -1.5f), v);
    }

    [Fact]
    private static void TestToString()
    {
        Assert.Equal("[1 2]", new Vector2DF(1f, 2f).ToString());
        Assert.Equal("[1.03125 2.5]", new Vector2DF(1.03125f, 2.5f).ToString());
    }
}
