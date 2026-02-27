using Xunit;

using UI.GFX.Geometry;

using static UI.Tests.GeometryUtil;
using UI.Extensions;

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

    [Fact]
    private static void TestContainsXY()
    {
        Assert.False(new RectF().Contains(0, 0));
        RectF r = new(10, 20, 30, 40);
        Assert.False(r.Contains(0, 0));
        Assert.False(r.Contains(9.9999f, 20));
        Assert.False(r.Contains(10, 19.9999f));
        Assert.True(r.Contains(10, 20));
        Assert.True(r.Contains(39.9999f, 20));
        Assert.False(r.Contains(40, 20));
        Assert.True(r.Contains(10, 59.9999f));
        Assert.False(r.Contains(10, 60));
        Assert.True(r.Contains(39.9999f, 59.9999f));
        Assert.False(r.Contains(40, 60));
        Assert.False(r.Contains(100, 100));
    }

    [Fact]
    private static void TestInclusiveContainsPointF()
    {
        Assert.True(new RectF().InclusiveContains(new PointF()));
        Assert.False(new RectF().InclusiveContains(new PointF(0.0001f, 0)));
        RectF r = new(10, 20, 30, 40);
        Assert.False(r.InclusiveContains(new PointF(0, 0)));
        Assert.False(r.InclusiveContains(new PointF(9.9999f, 20)));
        Assert.False(r.InclusiveContains(new PointF(10, 19.9999f)));
        Assert.True(r.InclusiveContains(new PointF(10, 20)));
        Assert.True(r.InclusiveContains(new PointF(40, 20)));
        Assert.False(r.InclusiveContains(new PointF(40.0001f, 20)));
        Assert.True(r.InclusiveContains(new PointF(10, 60)));
        Assert.False(r.InclusiveContains(new PointF(10, 60.0001f)));
        Assert.True(r.InclusiveContains(new PointF(40, 60)));
        Assert.False(r.InclusiveContains(new PointF(100, 100)));
    }

    [Fact]
    private static void TestInclusiveContainsXY()
    {
        Assert.True(new RectF().InclusiveContains(0, 0));
        Assert.False(new RectF().InclusiveContains(0.0001f, 0));
        RectF r = new(10, 20, 30, 40);
        Assert.False(r.InclusiveContains(0, 0));
        Assert.False(r.InclusiveContains(9.9999f, 20));
        Assert.False(r.InclusiveContains(10, 19.9999f));
        Assert.True(r.InclusiveContains(10, 20));
        Assert.True(r.InclusiveContains(40, 20));
        Assert.False(r.InclusiveContains(40.0001f, 20));
        Assert.True(r.InclusiveContains(10, 60));
        Assert.False(r.InclusiveContains(10, 60.0001f));
        Assert.True(r.InclusiveContains(40, 60));
        Assert.False(r.InclusiveContains(100, 100));
    }

    [Fact]
    private static void TestBoundingRect()
    {
        // If point B dominates A, then A should be the origin.
        AssertRectFEqual(new RectF(4.2f, 6.8f, 0, 0), RectF.BoundingRect(new PointF(4.2f, 6.8f), new PointF(4.2f, 6.8f)));
        AssertRectFEqual(new RectF(4.2f, 6.8f, 4.3f, 0), RectF.BoundingRect(new PointF(4.2f, 6.8f), new PointF(8.5f, 6.8f)));
        AssertRectFEqual(new RectF(4.2f, 6.8f, 0, 2.5f), RectF.BoundingRect(new PointF(4.2f, 6.8f), new PointF(4.2f, 9.3f)));
        AssertRectFEqual(new RectF(4.2f, 6.8f, 4.3f, 2.5f), RectF.BoundingRect(new PointF(4.2f, 6.8f), new PointF(8.5f, 9.3f)));
        // If point A dominates B, then B should be the origin.
        AssertRectFEqual(new RectF(4.2f, 6.8f, 0, 0), RectF.BoundingRect(new PointF(4.2f, 6.8f), new PointF(4.2f, 6.8f)));
        AssertRectFEqual(new RectF(4.2f, 6.8f, 4.3f, 0), RectF.BoundingRect(new PointF(8.5f, 6.8f), new PointF(4.2f, 6.8f)));
        AssertRectFEqual(new RectF(4.2f, 6.8f, 0, 2.5f), RectF.BoundingRect(new PointF(4.2f, 9.3f), new PointF(4.2f, 6.8f)));
        AssertRectFEqual(new RectF(4.2f, 6.8f, 4.3f, 2.5f), RectF.BoundingRect(new PointF(8.5f, 9.3f), new PointF(4.2f, 6.8f)));
        // If neither point dominates, then the origin is a combination of the two.
        AssertRectFEqual(new RectF(4.2f, 4.2f, 2.6f, 2.6f), RectF.BoundingRect(new PointF(4.2f, 6.8f), new PointF(6.8f, 4.2f)));
        AssertRectFEqual(new RectF(-6.8f, -6.8f, 2.6f, 2.6f), RectF.BoundingRect(new PointF(-4.2f, -6.8f), new PointF(-6.8f, -4.2f)));
        AssertRectFEqual(new RectF(-4.2f, -4.2f, 11.0f, 11.0f), RectF.BoundingRect(new PointF(-4.2f, 6.8f), new PointF(6.8f, -4.2f)));
        // If Point A and point B are far apart such that the exact result of
        // A.x - B.x cannot be presented by float, then the width or height increases
        // such that both A and B are included in the bounding rect.
        RectF boundingRect1 = RectF.BoundingRect(new PointF(20.0f, -1000000000.0f), new PointF(20.0f, 10.0f));
        Assert.True(boundingRect1.InclusiveContains(new PointF(20.0f, -1000000000.0f)));
        Assert.True(boundingRect1.InclusiveContains(new PointF(20.0f, 10.0f)));
        RectF boundingRect2 = RectF.BoundingRect(new PointF(20.0f, 20.0f), new PointF(20.0f, 1000000000.0f));
        Assert.True(boundingRect2.InclusiveContains(new PointF(20.0f, 20.0f)));
        Assert.True(boundingRect2.InclusiveContains(new PointF(20.0f, 1000000000.0f)));
        RectF boundingRect3 = RectF.BoundingRect(new PointF(-1000000000.0f, 20.0f), new PointF(20.0f, 20.0f));
        Assert.True(boundingRect3.InclusiveContains(new PointF(-1000000000.0f, 20.0f)));
        Assert.True(boundingRect3.InclusiveContains(new PointF(20.0f, 20.0f)));
        RectF boundingRect4 = RectF.BoundingRect(new PointF(20.0f, 20.0f), new PointF(1000000000.0f, 20.0f));
        Assert.True(boundingRect4.InclusiveContains(new PointF(20.0f, 20.0f)));
        Assert.True(boundingRect4.InclusiveContains(new PointF(1000000000.0f, 20.0f)));
    }

    [Fact]
    private static void TestUnion()
    {
        AssertRectFEqual(new RectF(), RectF.UnionRects(new RectF(), new RectF()));
        AssertRectFEqual(
            new RectF(1.1f, 2.2f, 3.3f, 4.4f),
            RectF.UnionRects(new RectF(1.1f, 2.2f, 3.3f, 4.4f), new RectF(1.1f, 2.2f, 3.3f, 4.4f)));
        AssertRectFEqual(
            new RectF(0, 0, 8.8f, 11.0f),
            RectF.UnionRects(new RectF(0, 0, 3.3f, 4.4f), new RectF(3.3f, 4.4f, 5.5f, 6.6f)));
        AssertRectFEqual(
            new RectF(0, 0, 8.8f, 11.0f),
            RectF.UnionRects(new RectF(3.3f, 4.4f, 5.5f, 6.6f), new RectF(0, 0, 3.3f, 4.4f)));
        AssertRectFEqual(
            new RectF(0, 1.1f, 3.3f, 8.8f),
            RectF.UnionRects(new RectF(0, 1.1f, 3.3f, 4.4f), new RectF(0, 5.5f, 3.3f, 4.4f)));
        AssertRectFEqual(
            new RectF(0, 1.1f, 11.0f, 12.1f),
            RectF.UnionRects(new RectF(0, 1.1f, 3.3f, 4.4f), new RectF(4.4f, 5.5f, 6.6f, 7.7f)));
        AssertRectFEqual(
            new RectF(0, 1.1f, 11.0f, 12.1f),
            RectF.UnionRects(new RectF(4.4f, 5.5f, 6.6f, 7.7f), new RectF(0, 1.1f, 3.3f, 4.4f)));
        AssertRectFEqual(
            new RectF(2.2f, 3.3f, 4.4f, 5.5f),
            RectF.UnionRects(new RectF(8.8f, 9.9f, 0, 2.2f), new RectF(2.2f, 3.3f, 4.4f, 5.5f)));
        AssertRectFEqual(
            new RectF(2.2f, 3.3f, 4.4f, 5.5f),
            RectF.UnionRects(new RectF(2.2f, 3.3f, 4.4f, 5.5f), new RectF(8.8f, 9.9f, 2.2f, 0)));
    }

    [Fact]
    private static void TestUnionEvenIfEmpty()
    {
        AssertRectFEqual(new RectF(), RectF.UnionRectsEvenIfEmpty(new RectF(), new RectF()));
        AssertRectFEqual(new RectF(0, 0, 3.3f, 4.4f),
                        RectF.UnionRectsEvenIfEmpty(new RectF(), new RectF(3.3f, 4.4f, 0, 0)));
        AssertRectFEqual(new RectF(0, 0, 8.8f, 11.0f),
                        RectF.UnionRectsEvenIfEmpty(new RectF(0, 0, 3.3f, 4.4f),
                                              new RectF(3.3f, 4.4f, 5.5f, 6.6f)));
        AssertRectFEqual(new RectF(0, 0, 8.8f, 11.0f),
                        RectF.UnionRectsEvenIfEmpty(new RectF(3.3f, 4.4f, 5.5f, 6.6f),
                                              new RectF(0, 0, 3.3f, 4.4f)));
        AssertRectFEqual(new RectF(2.2f, 3.3f, 6.6f, 8.8f),
                        RectF.UnionRectsEvenIfEmpty(new RectF(8.8f, 9.9f, 0, 2.2f),
                                              new RectF(2.2f, 3.3f, 4.4f, 5.5f)));
        AssertRectFEqual(new RectF(2.2f, 3.3f, 8.8f, 6.6f),
                        RectF.UnionRectsEvenIfEmpty(new RectF(2.2f, 3.3f, 4.4f, 5.5f),
                                              new RectF(8.8f, 9.9f, 2.2f, 0)));
    }

    [Fact]
    private static void TestUnionEnsuresContainWithFloatingError()
    {
        for (float f = 0.1f; f < 5; f += 0.1f)
        {
            RectF r1 = new(1, 2, 3, 4);
            r1.Scale(f, f + 0.05f);
            RectF r2 = r1 + new Vector2DF(10.0f + f, f - 10.0f);
            RectF r3 = RectF.UnionRects(r1, r2);
            Assert.True(r3.Contains(r1));
            Assert.True(r3.Contains(r2));
        }
    }

    [Fact]
    private static void TestUnionIfEmptyResultTinySize()
    {
        RectF r1 = new(1e-15f, 0, 0, 0);
        RectF r2 = new(0, 1e-15f, 0, 0);
        RectF r3 = RectF.UnionRectsEvenIfEmpty(r1, r2);
        Assert.False(r3.IsEmpty());
        Assert.True(r3.Contains(r1));
        Assert.True(r3.Contains(r2));
    }

    [Fact]
    private static void TestUnionMaxRects()
    {
        float kMaxFloat = float.MaxValue;
        float kMinFloat = float.MinNormal;
        RectF r1 = new(kMinFloat, 0, kMaxFloat, kMaxFloat);
        RectF r2 = new(0, kMinFloat, kMaxFloat, kMaxFloat);
        // This should not trigger DCHECK failure.
        r1.Union(r2);
    }
}
