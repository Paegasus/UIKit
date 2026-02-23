using Xunit;

using UI.GFX.Geometry;

namespace UI.Tests;

public static class Vector3DFTest
{
    [Fact]
    private static void TestIsZero()
    {
        Vector3DF float_zero = new(0, 0, 0);
        Vector3DF float_nonzero = new(0.1f, -0.1f, 0.1f);

        Assert.True(float_zero.IsZero());
        Assert.False(float_nonzero.IsZero());
    }

    [Fact]
    private static void TestAdd()
    {
        Vector3DF f1 = new(3.1f, 5.1f, 2.7f);
        Vector3DF f2 = new(4.3f, -1.3f, 8.1f);

        (Vector3DF expected, Vector3DF actual)[] values =
        [
            (new Vector3DF(3.1F, 5.1F, 2.7f), f1 + new Vector3DF()),
            (new Vector3DF(3.1f + 4.3f, 5.1f - 1.3f, 2.7f + 8.1f), f1 + f2),
            (new Vector3DF(3.1f - 4.3f, 5.1f + 1.3f, 2.7f - 8.1f), f1 - f2)
        ];

        foreach (var (expected, actual) in values)
        {
            Assert.Equal(expected.ToString(), actual.ToString());
        }
    }

    [Fact]
    private static void TestNegative()
    {
        (Vector3DF expected, Vector3DF actual)[] values =
        [
            (new Vector3DF(-0.0f, -0.0f, -0.0f), -new Vector3DF(0, 0, 0)),
            (new Vector3DF(-0.3f, -0.3f, -0.3f), -new Vector3DF(0.3f, 0.3f, 0.3f)),
            (new Vector3DF(0.3f, 0.3f, 0.3f), -new Vector3DF(-0.3f, -0.3f, -0.3f)),
            (new Vector3DF(-0.3f, 0.3f, -0.3f), -new Vector3DF(0.3f, -0.3f, 0.3f)),
            (new Vector3DF(0.3f, -0.3f, -0.3f), -new Vector3DF(-0.3f, 0.3f, 0.3f)),
            (new Vector3DF(-0.3f, -0.3f, 0.3f), -new Vector3DF(0.3f, 0.3f, -0.3f))
        ];

        foreach (var (expected, actual) in values)
            Assert.Equal(expected.ToString(), actual.ToString());
    }

    [Fact]
    private static void TestScale()
    {
        (float, float, float, float, float, float)[] triple_values =
        [
            (4.5f, 1.2f, 1.8f, 3.3f, 5.6f, 4.2f),
            (4.5f, -1.2f, -1.8f, 3.3f, 5.6f, 4.2f),
            (4.5f, 1.2f, -1.8f, 3.3f, 5.6f, 4.2f),
            (4.5f, -1.2f, 1.8f, 3.3f, 5.6f, 4.2f),

            (4.5f, 1.2f, 1.8f, 3.3f, -5.6f, -4.2f),
            (4.5f, 1.2f, 1.8f, -3.3f, -5.6f, -4.2f),
            (4.5f, 1.2f, -1.8f, 3.3f, -5.6f, -4.2f),
            (4.5f, 1.2f, -1.8f, -3.3f, -5.6f, -4.2f),

            (-4.5f, 1.2f, 1.8f, 3.3f, 5.6f, 4.2f),
            (-4.5f, 1.2f, 1.8f, 0, 5.6f, 4.2f),
            (-4.5f, 1.2f, -1.8f, 3.3f, 5.6f, 4.2f),
            (-4.5f, 1.2f, -1.8f, 0, 5.6f, 4.2f),

            (-4.5f, 1.2f, 1.8f, 3.3f, 0, 4.2f),
            (4.5f, 0, 1.8f, 3.3f, 5.6f, 4.2f),
            (-4.5f, 1.2f, -1.8f, 3.3f, 0, 4.2f),
            (4.5f, 0, -1.8f, 3.3f, 5.6f, 4.2f),
            (-4.5f, 1.2f, 1.8f, 3.3f, 5.6f, 0),
            (-4.5f, 1.2f, -1.8f, 3.3f, 5.6f, 0),

            (0, 1.2f, 0, 3.3f, 5.6f, 4.2f),
            (0, 1.2f, 1.8f, 3.3f, 5.6f, 4.2f)
        ];

        foreach (var (ONE, TWO, THREE, FOUR, FIVE, SIX) in triple_values)
        {
            Vector3DF v = new(ONE, TWO, THREE);
            v.Scale(FOUR, FIVE, SIX);
            Assert.Equal(ONE * FOUR, v.X);
            Assert.Equal(TWO * FIVE, v.Y);
            Assert.Equal(THREE * SIX, v.Z);

            Vector3DF v2 = Vector3DF.ScaleVector3D(new Vector3DF(ONE, TWO, THREE), FOUR, FIVE, SIX);
            Assert.Equal(ONE * FOUR, v2.X);
            Assert.Equal(TWO * FIVE, v2.Y);
            Assert.Equal(THREE * SIX, v2.Z);
        }

        (float, float, float, float)[] single_values =
        [
            (4.5f, 1.2f, 1.8f, 3.3f),
            (4.5f, -1.2f, 1.8f, 3.3f),
            (4.5f, 1.2f, -1.8f, 3.3f),
            (4.5f, -1.2f, -1.8f, 3.3f),
            (-4.5f, 1.2f, 3.3f, 0),
            (-4.5f, 1.2f, 0, 0),
            (-4.5f, 1.2f, 1.8f, 3.3f),
            (-4.5f, 1.2f, 1.8f, 0),
            (4.5f, 0, 1.8f, 3.3f),
            (0, 1.2f, 1.8f, 3.3f),
            (4.5f, 0, 1.8f, 3.3f),
            (0, 1.2f, 1.8f, 3.3f),
            (4.5f, 1.2f, 0, 3.3f),
            (4.5f, 1.2f, 0, 3.3f)
        ];

        foreach(var (ONE, TWO, THREE, FOUR) in single_values)
        {
            Vector3DF v = new(ONE, TWO, THREE);
            v.Scale(FOUR);
            Assert.Equal(ONE * FOUR, v.X);
            Assert.Equal(TWO * FOUR, v.Y);
            Assert.Equal(THREE * FOUR, v.Z);

            Vector3DF v2 = Vector3DF.ScaleVector3D(new Vector3DF(ONE, TWO, THREE), FOUR);
            Assert.Equal(ONE * FOUR, v2.X);
            Assert.Equal(TWO * FOUR, v2.Y);
            Assert.Equal(THREE * FOUR, v2.Z);
        }
    }

    [Fact]
    private static void TestLength()
    {
        float kFloatTolerance = 1e-7f;
        double kDoubleTolerance = 1e-15;

        (float, float, float)[] float_values =
        [
            (0, 0, 0),
            (10.5f, 20.5f, 8.5f),
            (20.5f, 10.5f, 8.5f),
            (8.5f, 20.5f, 10.5f),
            (10.5f, 8.5f, 20.5f),
            (-10.5f, -20.5f, -8.5f),
            (-20.5f, 10.5f, -8.5f),
            (-8.5f, -20.5f, -10.5f),
            (-10.5f, -8.5f, -20.5f),
            (10.5f, -20.5f, 8.5f),
            (-10.5f, 20.5f, 8.5f),
            (10.5f, -20.5f, -8.5f),
            (-10.5f, 20.5f, -8.5f),
            // A large vector that fails if the Length function doesn't use
            // double precision internally.
            (1236278317862780234892374893213178027.12122348904204230f,
            335890352589839028212313231225425134332.38123f,
            27861786423846742743236423478236784678.236713617231f)
        ];

        foreach (var (v0, v1, v2) in float_values)
        {
            double length_squared =
                (double)v0 * v0 +
                (double)v1 * v1 +
                (double)v2 * v2;
            
            double length = Math.Sqrt(length_squared);

            Vector3DF vector = new(v0, v1, v2);
            
            Assert.Equal(length_squared, vector.LengthSquared(), kDoubleTolerance);
            Assert.Equal((float)length, vector.Length(), kFloatTolerance);
        }
    }

    [Fact]
    private static void TestDotProduct()
    {
        (float, Vector3DF, Vector3DF)[] tests =
        [
            (0, new Vector3DF(1, 0, 0), new Vector3DF(0, 1, 1)),
            (0, new Vector3DF(0, 1, 0), new Vector3DF(1, 0, 1)),
            (0, new Vector3DF(0, 0, 1), new Vector3DF(1, 1, 0)),

            (3, new Vector3DF(1, 1, 1), new Vector3DF(1, 1, 1)),

            (1.2f, new Vector3DF(1.2f, -1.2f, 1.2f), new Vector3DF(1, 1, 1)),
            (1.2f, new Vector3DF(1, 1, 1), new Vector3DF(1.2f, -1.2f, 1.2f)),

            (38.72f, new Vector3DF(1.1f, 2.2f, 3.3f), new Vector3DF(4.4f, 5.5f, 6.6f))
        ];

        foreach (var (expected, input1, input2) in tests)
        {
            float actual = Vector3DF.DotProduct(input1, input2);
            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    private static void TestCrossProduct()
    {
        (Vector3DF, Vector3DF, Vector3DF)[] tests =
        [
            (new Vector3DF(), new Vector3DF(), new Vector3DF(1, 1, 1)),
            (new Vector3DF(), new Vector3DF(1, 1, 1), new Vector3DF()),
            (new Vector3DF(), new Vector3DF(1, 1, 1), new Vector3DF(1, 1, 1)),
            (new Vector3DF(), new Vector3DF(1.6f, 10.6f, -10.6f), new Vector3DF(1.6f, 10.6f, -10.6f)),

            (new Vector3DF(1, -1, 0), new Vector3DF(1, 1, 1), new Vector3DF(0, 0, 1)),
            (new Vector3DF(-1, 0, 1), new Vector3DF(1, 1, 1), new Vector3DF(0, 1, 0)),
            (new Vector3DF(0, 1, -1), new Vector3DF(1, 1, 1), new Vector3DF(1, 0, 0)),

            (new Vector3DF(-1, 1, 0), new Vector3DF(0, 0, 1), new Vector3DF(1, 1, 1)),
            (new Vector3DF(1, 0, -1), new Vector3DF(0, 1, 0), new Vector3DF(1, 1, 1)),
            (new Vector3DF(0, -1, 1), new Vector3DF(1, 0, 0), new Vector3DF(1, 1, 1))
        ];

        foreach (var (expected, input1, input2) in tests)
        {
            Vector3DF actual = Vector3DF.CrossProduct(input1, input2);
            Assert.Equal(expected.ToString(), actual.ToString());
        }
    }

    [Fact]
    private static void TestClampVector3dF()
    {
        Vector3DF a;

        a = new Vector3DF(3.5f, 5.5f, 7.5f);
        Assert.Equal(new Vector3DF(3.5f, 5.5f, 7.5f).ToString(), a.ToString());
        a.SetToMax(new Vector3DF(2, 4.5f, 6.5f));
        Assert.Equal(new Vector3DF(3.5f, 5.5f, 7.5f).ToString(), a.ToString());
        a.SetToMax(new Vector3DF(3.5f, 5.5f, 7.5f));
        Assert.Equal(new Vector3DF(3.5f, 5.5f, 7.5f).ToString(), a.ToString());
        a.SetToMax(new Vector3DF(4.5f, 2, 6.5f));
        Assert.Equal(new Vector3DF(4.5f, 5.5f, 7.5f).ToString(), a.ToString());
        a.SetToMax(new Vector3DF(3.5f, 6.5f, 6.5f));
        Assert.Equal(new Vector3DF(4.5f, 6.5f, 7.5f).ToString(), a.ToString());
        a.SetToMax(new Vector3DF(3.5f, 5.5f, 8.5f));
        Assert.Equal(new Vector3DF(4.5f, 6.5f, 8.5f).ToString(), a.ToString());
        a.SetToMax(new Vector3DF(8.5f, 10.5f, 12.5f));
        Assert.Equal(new Vector3DF(8.5f, 10.5f, 12.5f).ToString(), a.ToString());

        a.SetToMin(new Vector3DF(9.5f, 11.5f, 13.5f));
        Assert.Equal(new Vector3DF(8.5f, 10.5f, 12.5f).ToString(), a.ToString());
        a.SetToMin(new Vector3DF(8.5f, 10.5f, 12.5f));
        Assert.Equal(new Vector3DF(8.5f, 10.5f, 12.5f).ToString(), a.ToString());
        a.SetToMin(new Vector3DF(7.5f, 11.5f, 13.5f));
        Assert.Equal(new Vector3DF(7.5f, 10.5f, 12.5f).ToString(), a.ToString());
        a.SetToMin(new Vector3DF(9.5f, 9.5f, 13.5f));
        Assert.Equal(new Vector3DF(7.5f, 9.5f, 12.5f).ToString(), a.ToString());
        a.SetToMin(new Vector3DF(9.5f, 11.5f, 11.5f));
        Assert.Equal(new Vector3DF(7.5f, 9.5f, 11.5f).ToString(), a.ToString());
        a.SetToMin(new Vector3DF(3.5f, 5.5f, 7.5f));
        Assert.Equal(new Vector3DF(3.5f, 5.5f, 7.5f).ToString(), a.ToString());
    }

    [Fact]
    private static void TestAngleBetweenVectorsInDegress()
    {
        float kTolerance = 1e-7f;

        (float, Vector3DF, Vector3DF)[] tests =
        [
            (0, new Vector3DF(0, 1, 0), new Vector3DF(0, 1, 0)),
            (90, new Vector3DF(0, 1, 0), new Vector3DF(0, 0, 1)),
            (45, new Vector3DF(0, 1, 0),
            new Vector3DF(0, 0.70710678188f, 0.70710678188f)),
            (180, new Vector3DF(0, 1, 0), new Vector3DF(0, -1, 0)),
            // Two vectors that are sufficiently close enough together to
            // trigger an issue that produces NANs if the value passed to
            // acos is not clamped due to floating point precision.
            (0, new Vector3DF(0, -0.990842f, -0.003177f),
            new Vector3DF(0, -0.999995f, -0.003124f)),
        ];

        foreach (var (expected, input1, input2) in tests)
        {
            float actual = Vector3DF.AngleBetweenVectorsInDegrees(input1, input2);
            Assert.Equal(expected, actual, kTolerance);
            actual = Vector3DF.AngleBetweenVectorsInDegrees(input2, input1);
            Assert.Equal(expected, actual, kTolerance);
        }
    }

    [Fact]
    private static void TestClockwiseAngleBetweenVectorsInDegress()
    {
        float kTolerance = 1e-7f;

        (float, Vector3DF, Vector3DF)[] tests =
        [
            (0, new Vector3DF(0, 1, 0), new Vector3DF(0, 1, 0)),
            (90, new Vector3DF(0, 1, 0), new Vector3DF(0, 0, -1)),
            (45, new Vector3DF(0, -1, 0), new Vector3DF(0, -0.70710678188f, 0.70710678188f)),
            (180, new Vector3DF(0, -1, 0), new Vector3DF(0, 1, 0)),
            (270, new Vector3DF(0, 1, 0), new Vector3DF(0, 0, 1))
        ];
        
        Vector3DF normal_vector = new(1.0f, 0.0f, 0.0f);

        foreach(var (expected, input1, input2) in tests)
        {
            float actual = Vector3DF.ClockwiseAngleBetweenVectorsInDegrees(input1, input2, normal_vector);
            
            Assert.Equal(expected, actual, kTolerance);
            
            actual = -Vector3DF.ClockwiseAngleBetweenVectorsInDegrees(input2, input1, normal_vector);

            if (actual < 0.0f)
                actual += 360.0f;

            Assert.Equal(expected, actual, kTolerance);
        }
    }

    [Fact]
    private static void TestGetNormalized()
    {
        (bool, Vector3DF, Vector3DF)[] tests =
        [
            (false, new Vector3DF(0, 0, 0), new Vector3DF(0, 0, 0)),
            (false, new Vector3DF(float.Epsilon, float.Epsilon, float.Epsilon), new Vector3DF(float.Epsilon, float.Epsilon, float.Epsilon)),
            (true, new Vector3DF(1, 0, 0), new Vector3DF(1, 0, 0)),
            (true, new Vector3DF(float.MaxValue, 0, 0), new Vector3DF(1, 0, 0))
        ];

        foreach(var (expected, v, normalized) in tests)
        {
            Vector3DF n;
            Assert.Equal(expected, v.GetNormalized(out n));
            Assert.Equal(normalized.ToString(), n.ToString());
        }
    }

    [Fact]
    private static void TestToString()
    {
        Assert.Equal("[1.03125 2.5 -3]", new Vector3DF(1.03125f, 2.5f, -3f).ToString());
    }
}
