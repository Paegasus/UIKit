using Xunit;

using UI.GFX.Geometry;

namespace UI.Tests;

public static class Point3FTest
{
    [Fact]
    private static void TestVectorArithmetic()
    {
        Point3F a = new(1.6f, 5.1f, 3.2f);
        Vector3DF v1 = new(3.1f, -3.2f, 9.3f);
        Vector3DF v2 = new(-8.1f, 1.2f, 3.3f);

        (Point3F expected, Point3F actual)[] tests =
        [
            (new Point3F(4.7f, 1.9f, 12.5f), a + v1),
            (new Point3F(-1.5f, 8.3f, -6.1f), a - v1),
            (a, a - v1 + v1),
            (a, a + v1 - v1),
            (a, a + new Vector3DF()),
            (new Point3F(12.8f, 0.7f, 9.2f), a + v1 - v2),
            (new Point3F(-9.6f, 9.5f, -2.8f), a - v1 + v2)
        ];

        foreach (var (expected, actual) in tests)
            Assert.Equal(expected.ToString(), actual.ToString());

        a += v1;
        Assert.Equal(new Point3F(4.7f, 1.9f, 12.5f).ToString(), a.ToString());

        a -= v2;
        Assert.Equal(new Point3F(12.8f, 0.7f, 9.2f).ToString(), a.ToString());
    }

    [Fact]
    private static void TestVectorFromPoints()
    {
        Point3F a = new(1.6f, 5.2f, 3.2f);
        Vector3DF v1 = new(3.1f, -3.2f, 9.3f);

        Point3F b = a + v1;
        Assert.Equal((b - a).ToString(), v1.ToString());
    }

    [Fact]
    private static void TestScale()
    {
        Assert.Equal(new Point3F().ToString(), Point3F.ScalePoint(new Point3F(), 2.0f).ToString());
        Assert.Equal(new Point3F().ToString(),
                  Point3F.ScalePoint(new Point3F(), 2.0f, 2.0f, 2.0f).ToString());

        Assert.Equal(new Point3F(2.0f, -2.0f, 4.0f).ToString(),
                  Point3F.ScalePoint(new Point3F(1.0f, -1.0f, 2.0f), 2.0f).ToString());
        Assert.Equal(new Point3F(2.0f, -3.0f, 8.0f).ToString(),
                  Point3F.ScalePoint(new Point3F(1.0f, -1.0f, 2.0f), 2.0f, 3.0f, 4.0f).ToString());

        Point3F zero = new();
        zero.Scale(2.0f);
        zero.Scale(6.0f, 3.0f, 1.5f);
        Assert.Equal(new Point3F().ToString(), zero.ToString());

        Point3F point = new(1.0f, -1.0f, 2.0f);
        point.Scale(2.0f);
        point.Scale(6.0f, 3.0f, 1.5f);
        Assert.Equal(new Point3F(12.0f, -6.0f, 6.0f).ToString(), point.ToString());
    }

    [Fact]
    private static void TestIsOrigin()
    {
        Assert.True(new Point3F().IsOrigin());
        Assert.False(new Point3F(0, 0, 0.1f).IsOrigin());
        Assert.False(new Point3F(0, 0.1f, 0).IsOrigin());
        Assert.False(new Point3F(0.1f, 0, 0).IsOrigin());
        Assert.False(new Point3F(0, 0, -0.1f).IsOrigin());
        Assert.False(new Point3F(0, -0.1f, 0).IsOrigin());
        Assert.False(new Point3F(-0.1f, 0, 0).IsOrigin());
    }

    [Fact]
    private static void TestOffsetFromOrigin()
    {
        Assert.Equal(new Vector3DF(1.25f, 2.5f, -3.75f),
            new Point3F(1.25f, 2.5f, -3.75f).OffsetFromOrigin());
    }

    [Fact]
    private static void TestToString()
    {
        Assert.Equal("1.03125,2.5,-3", new Point3F(1.03125f, 2.5f, -3f).ToString());
    }
}
