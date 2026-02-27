using Xunit;

using UI.GFX.Geometry;
using UI.Extensions;

using static UI.GFX.Geometry.RectConversions;

namespace UI.Tests;

public static class RectFTest
{
    [Fact]
    private static void TestFromRect()
    {
        // Check that explicit conversion from integer to float compiles.
        Rect a = new(10, 20, 30, 40);
        RectF b = new(10, 20, 30, 40);

        RectF c = new RectF(a);
        Assert.Equal(b, c);
    }

    [Fact]
    private static void TestContainsPointF()
    {
        Assert.False(new RectF().Contains(new PointF()));
        RectF r = new(10, 20, 30, 40);
        Assert.False(r.Contains(new PointF(0, 0)));
        Assert.False(r.Contains(new PointF(9.9999f, 20)));
        Assert.False(r.Contains(new PointF(10, 19.9999f)));
        Assert.True(r.Contains(new PointF(10, 20)));
        Assert.True(r.Contains(new PointF(39.9999f, 20)));
        Assert.False(r.Contains(new PointF(40, 20)));
        Assert.True(r.Contains(new PointF(10, 59.9999f)));
        Assert.False(r.Contains(new PointF(10, 60)));
        Assert.True(r.Contains(new PointF(39.9999f, 59.9999f)));
        Assert.False(r.Contains(new PointF(40, 60)));
        Assert.False(r.Contains(new PointF(100, 100)));
    }
}
