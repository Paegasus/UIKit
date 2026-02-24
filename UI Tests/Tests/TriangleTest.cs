using Xunit;

using UI.GFX.Geometry;

using static UI.GFX.Geometry.Triangle;

namespace UI.Tests;

public static class TriangleTest
{
    private static PointF kPointA = new(1, 1);
    private static PointF kPointB = new(10, 1);
    private static PointF kPointC = new(1, 10);

    [Fact]
    private static void TestPointIsInTriangleInside()
    {
        PointF p = new(2, 2);

        Assert.True(PointIsInTriangle(p, kPointA, kPointB, kPointC));
    }

    [Fact]
    private static void TestPointIsInTriangleOutside()
    {
        PointF o = new(0, 0);

        Assert.False(PointIsInTriangle(o, kPointA, kPointB, kPointC));
    }

    [Fact]
    private static void TestPointIsInTriangleEdge()
    {
        PointF e = new(1, 3);

        Assert.True(PointIsInTriangle(e, kPointA, kPointB, kPointC));
    }

    [Fact]
    private static void TestPointIsInTriangleVertex()
    {
        Assert.True(PointIsInTriangle(kPointA, kPointA, kPointB, kPointC));
    }
}
