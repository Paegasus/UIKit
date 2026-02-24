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
}
