using Xunit;

using UI.GFX.Geometry;

using static UI.Tests.GeometryUtil;
using UI.Extensions;

namespace UI.Tests;

public static class RectTest
{
    private static int kMaxInt = int.MaxValue;
    private static int kMinInt = int.MinValue;

    [Fact]
    private static void TestContains()
    {
        (int rect_x, int rect_y, int rect_width, int rect_height, int point_x, int point_y, bool contained)[] contains_cases =
        [
            (0, 0, 10, 10, 0, 0, true),
            (0, 0, 10, 10, 5, 5, true),
            (0, 0, 10, 10, 9, 9, true),
            (0, 0, 10, 10, 5, 10, false),
            (0, 0, 10, 10, 10, 5, false),
            (0, 0, 10, 10, -1, -1, false),
            (0, 0, 10, 10, 50, 50, false),
            (0, 0, -10, -10, 0, 0, false)
        ];

        foreach(var (rect_x, rect_y, rect_width, rect_height, point_x, point_y, contained) in contains_cases)
        {
            Rect rect = new(rect_x, rect_y, rect_width, rect_height);
            Assert.Equal(contained, rect.Contains(point_x, point_y));
        }
    }

    [Fact]
    private static void TestIntersects()
    {
        (
        // rect 1
        int x1, int y1, int w1, int h1,
        // rect 2
        int x2, int y2, int w2, int h2, bool intersects)[] tests =
        [
            (0, 0, 0, 0, 0, 0, 0, 0, false),
            (0, 0, 0, 0, -10, -10, 20, 20, false),
            (-10, 0, 0, 20, 0, -10, 20, 0, false),
            (0, 0, 10, 10, 0, 0, 10, 10, true),
            (0, 0, 10, 10, 10, 10, 10, 10, false),
            (10, 10, 10, 10, 0, 0, 10, 10, false),
            (10, 10, 10, 10, 5, 5, 10, 10, true),
            (10, 10, 10, 10, 15, 15, 10, 10, true),
            (10, 10, 10, 10, 20, 15, 10, 10, false),
            (10, 10, 10, 10, 21, 15, 10, 10, false)
        ];

        foreach(var (x1, y1, w1, h1, x2, y2, w2, h2, intersects) in tests)
        {
            Rect r1 = new(x1, y1, w1, h1);
            Rect r2 = new(x2, y2, w2, h2);
            Assert.Equal(intersects, r1.Intersects(r2));
            Assert.Equal(intersects, r2.Intersects(r1));
        }
    }

    [Fact]
    private static void TestIntersect()
    {
        (
        // rect 1
        int x1, int y1, int w1, int h1,
        // rect 2
        int x2, int y2, int w2, int h2,
        // rect 3: the union of rects 1 and 2
        int x3, int y3, int w3, int h3)[] tests =
        [
            (0, 0, 0, 0,  // zeros
            0, 0, 0, 0, 0, 0, 0, 0),
            (0, 0, 4, 4,  // equal
            0, 0, 4, 4, 0, 0, 4, 4),
            (0, 0, 4, 4,  // neighboring
            4, 4, 4, 4, 0, 0, 0, 0),
            (0, 0, 4, 4,  // overlapping corners
            2, 2, 4, 4, 2, 2, 2, 2),
            (0, 0, 4, 4,  // T junction
            3, 1, 4, 2, 3, 1, 1, 2),
            (3, 0, 2, 2,  // gap
            0, 0, 2, 2, 0, 0, 0, 0)
        ];

        foreach(var (x1, y1, w1, h1, x2, y2, w2, h2, x3, y3, w3, h3) in tests)
        {
            Rect r1 = new(x1, y1, w1, h1);
            Rect r2 = new(x2, y2, w2, h2);
            Rect r3 = new(x3, y3, w3, h3);
            Assert.Equal(r3, Rect.IntersectRects(r1, r2));
        }
    }

    [Fact]
    private static void TestInclusiveIntersect()
    {
        Rect rect = new(11, 12, 0, 0);
        Assert.True(rect.InclusiveIntersect(new Rect(11, 12, 13, 14)));
        Assert.Equal(new Rect(11, 12, 0, 0), rect);

        rect = new Rect(11, 12, 13, 14);
        Assert.True(rect.InclusiveIntersect(new Rect(24, 8, 0, 7)));
        Assert.Equal(new Rect(24, 12, 0, 3), rect);

        rect = new Rect(11, 12, 13, 14);
        Assert.True(rect.InclusiveIntersect(new Rect(9, 15, 4, 0)));
        Assert.Equal(new Rect(11, 15, 2, 0), rect);

        rect = new Rect(11, 12, 0, 14);
        Assert.False(rect.InclusiveIntersect(new Rect(12, 13, 15, 16)));
        Assert.Equal(new Rect(), rect);
    }

    [Fact]
    private static void TestUnion()
    {
        Assert.Equal(new Rect(), Rect.UnionRects(new Rect(), new Rect()));
        Assert.Equal(new Rect(1, 2, 3, 4), Rect.UnionRects(new Rect(1, 2, 3, 4), new Rect(1, 2, 3, 4)));
        Assert.Equal(new Rect(0, 0, 8, 10), Rect.UnionRects(new Rect(0, 0, 3, 4), new Rect(3, 4, 5, 6)));
        Assert.Equal(new Rect(0, 0, 8, 10), Rect.UnionRects(new Rect(3, 4, 5, 6), new Rect(0, 0, 3, 4)));
        Assert.Equal(new Rect(0, 1, 3, 8), Rect.UnionRects(new Rect(0, 1, 3, 4), new Rect(0, 5, 3, 4)));
        Assert.Equal(new Rect(0, 1, 10, 11), Rect.UnionRects(new Rect(0, 1, 3, 4), new Rect(4, 5, 6, 7)));
        Assert.Equal(new Rect(0, 1, 10, 11), Rect.UnionRects(new Rect(4, 5, 6, 7), new Rect(0, 1, 3, 4)));
        Assert.Equal(new Rect(2, 3, 4, 5), Rect.UnionRects(new Rect(8, 9, 0, 2), new Rect(2, 3, 4, 5)));
        Assert.Equal(new Rect(2, 3, 4, 5), Rect.UnionRects(new Rect(2, 3, 4, 5), new Rect(8, 9, 2, 0)));
    }

    [Fact]
    private static void TestUnionEvenIfEmpty()
    {
        Assert.Equal(new Rect(), Rect.UnionRectsEvenIfEmpty(new Rect(), new Rect()));
        Assert.Equal(new Rect(0, 0, 3, 4), Rect.UnionRectsEvenIfEmpty(new Rect(), new Rect(3, 4, 0, 0)));
        Assert.Equal(new Rect(0, 0, 8, 10),
                Rect.UnionRectsEvenIfEmpty(new Rect(0, 0, 3, 4), new Rect(3, 4, 5, 6)));
        Assert.Equal(new Rect(0, 0, 8, 10),
                Rect.UnionRectsEvenIfEmpty(new Rect(3, 4, 5, 6), new Rect(0, 0, 3, 4)));
        Assert.Equal(new Rect(2, 3, 6, 8),
                Rect.UnionRectsEvenIfEmpty(new Rect(8, 9, 0, 2), new Rect(2, 3, 4, 5)));
        Assert.Equal(new Rect(2, 3, 8, 6),
                Rect.UnionRectsEvenIfEmpty(new Rect(2, 3, 4, 5), new Rect(8, 9, 2, 0)));
    }

    [Fact]
    private static void TestEquals()
    {
        Assert.True(new Rect(0, 0, 0, 0) == new Rect(0, 0, 0, 0));
        Assert.True(new Rect(1, 2, 3, 4) == new Rect(1, 2, 3, 4));
        Assert.False(new Rect(0, 0, 0, 0) == new Rect(0, 0, 0, 1));
        Assert.False(new Rect(0, 0, 0, 0) == new Rect(0, 0, 1, 0));
        Assert.False(new Rect(0, 0, 0, 0) == new Rect(0, 1, 0, 0));
        Assert.False(new Rect(0, 0, 0, 0) == new Rect(1, 0, 0, 0));
    }
}
