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

    [Fact]
    private static void TestAddingVectors()
    {
        PointF a = new(1, 1);
        PointF b = new(2, 1);
        PointF c = new(2, 2);
        PointF d = new(1, 2);
        Vector2DF v = new(3.5f, -2.5f);

        QuadF q1 = new(a, b, c, d);
        QuadF added = q1 + v;
        q1 += v;
        var expected1 = new QuadF(new PointF(4.5f, -1.5f), new PointF(5.5f, -1.5f), new PointF(5.5f, -0.5f), new PointF(4.5f, -0.5f));
        Assert.Equal(expected1, added);
        Assert.Equal(expected1, q1);

        QuadF q2 = new(a, b, c, d);
        QuadF subtracted = q2 - v;
        q2 -= v;
        var expected2 = new QuadF(new PointF(-2.5f, 3.5f), new PointF(-1.5f, 3.5f), new PointF(-1.5f, 4.5f), new PointF(-2.5f, 4.5f));
        Assert.Equal(expected2, subtracted);
        Assert.Equal(expected2, q2);

        QuadF q3 = new(a, b, c, d);
        q3 += v;
        q3 -= v;
        Assert.Equal(new QuadF(a, b, c, d), q3);
        Assert.Equal(q3, (q3 + v - v));
    }
}
