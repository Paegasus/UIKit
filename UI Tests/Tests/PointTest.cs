using Xunit;

using UI.GFX.Geometry;

using static UI.GFX.Geometry.PointConversions;

namespace UI.Tests;

public static class PointTest
{
    [Fact]
    private static void TestIsOrigin()
    {
        Assert.False(new Point(1, 0).IsOrigin());
        Assert.False(new Point(0, 1).IsOrigin());
        Assert.False(new Point(1, 2).IsOrigin());
        Assert.False(new Point(-1, 0).IsOrigin());
        Assert.False(new Point(0, -1).IsOrigin());
        Assert.False(new Point(-1, -2).IsOrigin());
        Assert.True(new Point(0, 0).IsOrigin());
    }

    [Fact]
    private static void TestVectorArithmetic()
    {
        Point a = new(1, 5);
        Vector2D v1 = new(3, -3);
        Vector2D v2 = new(-8, 1);

        (Point expected, Point actual)[] tests =
        [
            ( new Point(4, 2), a + v1 ),
            ( new Point(-2, 8), a - v1 ),
            ( a, a - v1 + v1 ),
            ( a, a + v1 - v1 ),
            ( a, a + new Vector2D() ),
            ( new Point(12, 1), a + v1 - v2 ),
            ( new Point(-10, 9), a - v1 + v2 )
        ];

        foreach(var (expected, actual) in tests)
        {
            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    private static void TestOffsetFromPoint()
    {
        Point a = new(1, 5);
        Point b = new(-20, 8);
        Assert.Equal(new Vector2D(-20 - 1, 8 - 5), (b - a));
    }

    [Fact]
    private static void TestSetToMinMax()
    {
        Point a;

        a = new Point(3, 5);
        Assert.Equal(new Point(3, 5), a);
        a.SetToMax(new Point(2, 4));
        Assert.Equal(new Point(3, 5), a);
        a.SetToMax(new Point(3, 5));
        Assert.Equal(new Point(3, 5), a);
        a.SetToMax(new Point(4, 2));
        Assert.Equal(new Point(4, 5), a);
        a.SetToMax(new Point(8, 10));
        Assert.Equal(new Point(8, 10), a);

        a.SetToMin(new Point(9, 11));
        Assert.Equal(new Point(8, 10), a);
        a.SetToMin(new Point(8, 10));
        Assert.Equal(new Point(8, 10), a);
        a.SetToMin(new Point(11, 9));
        Assert.Equal(new Point(8, 9), a);
        a.SetToMin(new Point(7, 11));
        Assert.Equal(new Point(7, 9), a);
        a.SetToMin(new Point(3, 5));
        Assert.Equal(new Point(3, 5), a);
    }
}
