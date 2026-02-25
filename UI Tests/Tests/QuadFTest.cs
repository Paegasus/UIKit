using Xunit;

using UI.GFX.Geometry;

using static UI.GFX.Geometry.PointConversions;

namespace UI.Tests;

public static class QuadFTest
{
    [Fact]
    private static void TestConstruction()
    {
        // Verify constructors.
        PointF a = new(1, 1);
        PointF b = new(2, 1);
        PointF c = new(2, 2);
        PointF d = new(1, 2);
        PointF e = new();
        QuadF q1 = new();
        QuadF q2 = new(e, e, e, e);
        QuadF q3 = new(a, b, c, d);
        QuadF q4 = new(RectF.BoundingRect(a, c));
        Assert.Equal(q1, q2);
        Assert.Equal(q3, q4);

        // Verify getters.
        Assert.Equal(q3.p1, a);
        Assert.Equal(q3.p2, b);
        Assert.Equal(q3.p3, c);
        Assert.Equal(q3.p4, d);

        // Verify setters.
        q3.p1 = b;
        q3.p2 = c;
        q3.p3 = d;
        q3.p4 = a;
        Assert.Equal(q3.p1, b);
        Assert.Equal(q3.p2, c);
        Assert.Equal(q3.p3, d);
        Assert.Equal(q3.p4, a);

        // Verify operator=(Rect)
        Assert.NotEqual(q1, q4);
        q1 = RectF.BoundingRect(a, c);
        Assert.Equal(q1, q4);

        // Verify operator=(Quad)
        Assert.NotEqual(q1, q3);
        q1 = q3;
        Assert.Equal(q1, q3);
    }
}
