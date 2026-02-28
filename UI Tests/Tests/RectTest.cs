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

    [Fact]
    private static void TestAdjustToFit()
    {
        (
        // source
        int x1, int y1, int w1, int h1,
        // target
        int x2, int y2, int w2, int h2,
        // rect 3: results of invoking AdjustToFit
        int x3, int y3, int w3, int h3)[] tests =
        [
            (0, 0, 2, 2, 0, 0, 2, 2, 0, 0, 2, 2),
            (2, 2, 3, 3, 0, 0, 4, 4, 1, 1, 3, 3),
            (-1, -1, 5, 5, 0, 0, 4, 4, 0, 0, 4, 4),
            (2, 2, 4, 4, 0, 0, 3, 3, 0, 0, 3, 3),
            (2, 2, 1, 1, 0, 0, 3, 3, 2, 2, 1, 1)
        ];

        foreach (var (x1, y1, w1, h1, x2, y2, w2, h2, x3, y3, w3, h3) in tests)
        {
            Rect r1 = new(x1, y1, w1, h1);
            Rect r2 = new(x2, y2, w2, h2);
            Rect r3 = new(x3, y3, w3, h3);
            Rect u = r1;
            u.AdjustToFit(r2);
            Assert.Equal(r3, u);
        }
    }

    [Fact]
    private static void TestSubtract()
    {
        Rect result;

        // Matching
        result = new Rect(10, 10, 20, 20);
        result.Subtract(new Rect(10, 10, 20, 20));
        Assert.Equal(new Rect(0, 0, 0, 0), result);

        // Contains
        result = new Rect(10, 10, 20, 20);
        result.Subtract(new Rect(5, 5, 30, 30));
        Assert.Equal(new Rect(0, 0, 0, 0), result);

        // No intersection
        result = new Rect(10, 10, 20, 20);
        result.Subtract(new Rect(30, 30, 30, 30));
        Assert.Equal(new Rect(10, 10, 20, 20), result);

        // Not a complete intersection in either direction
        result = new Rect(10, 10, 20, 20);
        result.Subtract(new Rect(15, 15, 20, 20));
        Assert.Equal(new Rect(10, 10, 20, 20), result);

        // Complete intersection in the x-direction, top edge is fully covered.
        result = new Rect(10, 10, 20, 20);
        result.Subtract(new Rect(10, 15, 20, 20));
        Assert.Equal(new Rect(10, 10, 20, 5), result);

        // Complete intersection in the x-direction, top edge is fully covered.
        result = new Rect(10, 10, 20, 20);
        result.Subtract(new Rect(5, 15, 30, 20));
        Assert.Equal(new Rect(10, 10, 20, 5), result);

        // Complete intersection in the x-direction, bottom edge is fully covered.
        result = new Rect(10, 10, 20, 20);
        result.Subtract(new Rect(5, 5, 30, 20));
        Assert.Equal(new Rect(10, 25, 20, 5), result);

        // Complete intersection in the x-direction, none of the edges is fully
        // covered.
        result = new Rect(10, 10, 20, 20);
        result.Subtract(new Rect(5, 15, 30, 1));
        Assert.Equal(new Rect(10, 10, 20, 20), result);

        // Complete intersection in the y-direction, left edge is fully covered.
        result = new Rect(10, 10, 20, 20);
        result.Subtract(new Rect(10, 10, 10, 30));
        Assert.Equal(new Rect(20, 10, 10, 20), result);

        // Complete intersection in the y-direction, left edge is fully covered.
        result = new Rect(10, 10, 20, 20);
        result.Subtract(new Rect(5, 5, 20, 30));
        Assert.Equal(new Rect(25, 10, 5, 20), result);

        // Complete intersection in the y-direction, right edge is fully covered.
        result = new Rect(10, 10, 20, 20);
        result.Subtract(new Rect(20, 5, 20, 30));
        Assert.Equal(new Rect(10, 10, 10, 20), result);

        // Complete intersection in the y-direction, none of the edges is fully
        // covered.
        result = new Rect(10, 10, 20, 20);
        result.Subtract(new Rect(15, 5, 1, 30));
        Assert.Equal(new Rect(10, 10, 20, 20), result);
    }

    [Fact]
    private static void TestIsEmpty()
    {
        Assert.True(new Rect(0, 0, 0, 0).IsEmpty());
        Assert.True(new Rect(0, 0, 0, 0).Size.IsEmpty());
        Assert.True(new Rect(0, 0, 10, 0).IsEmpty());
        Assert.True(new Rect(0, 0, 10, 0).Size.IsEmpty());
        Assert.True(new Rect(0, 0, 0, 10).IsEmpty());
        Assert.True(new Rect(0, 0, 0, 10).Size.IsEmpty());
        Assert.False(new Rect(0, 0, 10, 10).IsEmpty());
        Assert.False(new Rect(0, 0, 10, 10).Size.IsEmpty());
    }

    [Fact]
    private static void TestSplitVertically()
    {
        Rect left_half, right_half;

        // Splitting when origin is (0, 0).
        new Rect(0, 0, 20, 20).SplitVertically(out left_half, out right_half);
        Assert.True(left_half == new Rect(0, 0, 10, 20));
        Assert.True(right_half == new Rect(10, 0, 10, 20));

        // Splitting when origin is arbitrary.
        new Rect(10, 10, 20, 10).SplitVertically(out left_half, out right_half);
        Assert.True(left_half == new Rect(10, 10, 10, 10));
        Assert.True(right_half == new Rect(20, 10, 10, 10));

        // Splitting a rectangle of zero width.
        new Rect(10, 10, 0, 10).SplitVertically(out left_half, out right_half);
        Assert.True(left_half == new Rect(10, 10, 0, 10));
        Assert.True(right_half == new Rect(10, 10, 0, 10));

        // Splitting a rectangle of odd width.
        new Rect(10, 10, 5, 10).SplitVertically(out left_half, out right_half);
        Assert.True(left_half == new Rect(10, 10, 2, 10));
        Assert.True(right_half == new Rect(12, 10, 3, 10));
    }

    [Fact]
    private static void TestSplitHorizontally()
    {
        Rect top_half, bottom_half;

        // Splitting when origin is (0, 0).
        new Rect(0, 0, 10, 20).SplitHorizontally(out top_half, out bottom_half);
        Assert.Equal(new Rect(0, 0, 10, 10), top_half);
        Assert.Equal(new Rect(0, 10, 10, 10), bottom_half);

        // Splitting when origin is arbitrary.
        new Rect(10, 10, 10, 20).SplitHorizontally(out top_half, out bottom_half);
        Assert.Equal(new Rect(10, 10, 10, 10), top_half);
        Assert.Equal(new Rect(10, 20, 10, 10), bottom_half);

        // Splitting a rectangle of zero height.
        new Rect(10, 10, 10, 0).SplitHorizontally(out top_half, out bottom_half);
        Assert.Equal(new Rect(10, 10, 10, 0), top_half);
        Assert.Equal(new Rect(10, 10, 10, 0), bottom_half);

        // Splitting a rectangle of odd height.
        new Rect(10, 10, 10, 5).SplitHorizontally(out top_half, out bottom_half);
        Assert.Equal(new Rect(10, 10, 10, 2), top_half);
        Assert.Equal(new Rect(10, 12, 10, 3), bottom_half);
    }

    [Fact]
    private static void TestCenterPoint()
    {
        Point center;

        // When origin is (0, 0).
        center = new Rect(0, 0, 20, 20).CenterPoint();
        Assert.True(center == new Point(10, 10));

        // When origin is even.
        center = new Rect(10, 10, 20, 20).CenterPoint();
        Assert.True(center == new Point(20, 20));

        // When origin is odd.
        center = new Rect(11, 11, 20, 20).CenterPoint();
        Assert.True(center == new Point(21, 21));

        // When 0 width or height.
        center = new Rect(10, 10, 0, 20).CenterPoint();
        Assert.True(center == new Point(10, 20));
        center = new Rect(10, 10, 20, 0).CenterPoint();
        Assert.True(center == new Point(20, 10));

        // When an odd size.
        center = new Rect(10, 10, 21, 21).CenterPoint();
        Assert.True(center == new Point(20, 20));

        // When an odd size and position.
        center = new Rect(11, 11, 21, 21).CenterPoint();
        Assert.True(center == new Point(21, 21));
    }

    [Fact]
    private static void TestSharesEdgeWith()
    {
        Rect r = new(2, 3, 4, 5);

        // Must be non-overlapping
        Assert.False(r.SharesEdgeWith(r));

        Rect just_above = new(2, 1, 4, 2);
        Rect just_below = new(2, 8, 4, 2);
        Rect just_left = new(0, 3, 2, 5);
        Rect just_right = new(6, 3, 2, 5);

        Assert.True(r.SharesEdgeWith(just_above));
        Assert.True(r.SharesEdgeWith(just_below));
        Assert.True(r.SharesEdgeWith(just_left));
        Assert.True(r.SharesEdgeWith(just_right));

        // Wrong placement
        Rect same_height_no_edge = new(0, 0, 1, 5);
        Rect same_width_no_edge = new(0, 0, 4, 1);

        Assert.False(r.SharesEdgeWith(same_height_no_edge));
        Assert.False(r.SharesEdgeWith(same_width_no_edge));

        Rect just_above_no_edge = new(2, 1, 5, 2);  // too wide
        Rect just_below_no_edge = new(2, 8, 3, 2);  // too narrow
        Rect just_left_no_edge = new(0, 3, 2, 6);   // too tall
        Rect just_right_no_edge = new(6, 3, 2, 4);  // too short

        Assert.False(r.SharesEdgeWith(just_above_no_edge));
        Assert.False(r.SharesEdgeWith(just_below_no_edge));
        Assert.False(r.SharesEdgeWith(just_left_no_edge));
        Assert.False(r.SharesEdgeWith(just_right_no_edge));
    }

    private static void TestScaleRectOverflowClamp(Func<Rect, float, float, Rect> function)
    {
        // The whole rect is scaled out of kMinInt.
        Rect xy_underflow1 = new(-100000, -123456, 10, 20);
        Assert.Equal(new Rect(kMinInt, kMinInt, 0, 0), function(xy_underflow1, 100000, 100000));

        // This rect's right/bottom is 0. The origin overflows, and is clamped to
        // -kMaxInt (instead of kMinInt) to keep width/height not overflowing.
        Rect xy_underflow2 = new(-100000, -123456, 100000, 123456);
        Assert.Equal(new Rect(-kMaxInt, -kMaxInt, kMaxInt, kMaxInt),
                  function(xy_underflow2, 100000, 100000));

        // A location overflow means that width/right and bottom/top also
        // overflow so need to be clamped.
        Rect xy_overflow = new(100000, 123456, 10, 20);
        Assert.Equal(new Rect(kMaxInt, kMaxInt, 0, 0),
                  function(xy_overflow, 100000, 100000));

        // In practice all rects are clamped to 0 width / 0 height so
        // negative sizes don't matter, but try this for the sake of testing.
        Rect size_underflow = new(-1, -2, 100000, 100000);
        Assert.Equal(new Rect(100000, 200000, 0, 0),
                  function(size_underflow, -100000, -100000));

        Rect size_overflow = new(-1, -2, 123456, 234567);
        Assert.Equal(new Rect(-100000, -200000, kMaxInt, kMaxInt),
                  function(size_overflow, 100000, 100000));
        // Verify width/right gets clamped properly too if x/y positive.
        Rect size_overflow2 = new(1, 2, 123456, 234567);
        Assert.Equal(new Rect(100000, 200000, kMaxInt - 100000, kMaxInt - 200000),
                  function(size_overflow2, 100000, 100000));

        float kMaxIntAsFloat = (float)kMaxInt;
        Rect max_origin_rect = new(kMaxInt, kMaxInt, kMaxInt, kMaxInt);
        // width/height of max_origin_rect has already been clamped to 0.
        Assert.Equal(new Rect(kMaxInt, kMaxInt, 0, 0), max_origin_rect);
        Assert.Equal(new Rect(kMaxInt, kMaxInt, 0, 0),
                  function(max_origin_rect, kMaxIntAsFloat, kMaxIntAsFloat));

        Rect max_size_rect1 = new(0, 0, kMaxInt, kMaxInt);
        // Max sized rect can't be scaled up any further in any dimension.
        Assert.Equal(max_size_rect1, function(max_size_rect1, 2, 3.5f));
        Assert.Equal(max_size_rect1,
                  function(max_size_rect1, kMaxIntAsFloat, kMaxIntAsFloat));
        // Max sized ret scaled by negative scale is an empty rect.
        Assert.Equal(new Rect(), function(max_size_rect1, kMinInt, kMinInt));

        Rect max_size_rect2 = new(-kMaxInt, -kMaxInt, kMaxInt, kMaxInt);
        Assert.Equal(max_size_rect2, function(max_size_rect2, 2, 3.5f));
        Assert.Equal(max_size_rect2,
                  function(max_size_rect2, kMaxIntAsFloat, kMaxIntAsFloat));
        Assert.Equal(new Rect(kMaxInt, kMaxInt, 0, 0),
                  function(max_size_rect2, kMinInt, kMinInt));
    }

    [Fact]
    private static void TestScaleToEnclosedRect()
    {
        Assert.Equal(new Rect(), Rect.ScaleToEnclosedRect(new Rect(), 5.0f));
        Assert.Equal(new Rect(5, 5, 5, 5), Rect.ScaleToEnclosedRect(new Rect(1, 1, 1, 1), 5.0f));
        Assert.Equal(new Rect(-5, -5, 0, 0), Rect.ScaleToEnclosedRect(new Rect(-1, -1, 0, 0), 5.0f));
        Assert.Equal(new Rect(5, -5, 0, 5), Rect.ScaleToEnclosedRect(new Rect(1, -1, 0, 1), 5.0f));
        Assert.Equal(new Rect(-5, 5, 5, 0), Rect.ScaleToEnclosedRect(new Rect(-1, 1, 1, 0), 5.0f));
        Assert.Equal(new Rect(2, 3, 4, 6), Rect.ScaleToEnclosedRect(new Rect(1, 2, 3, 4), 1.5f));
        Assert.Equal(new Rect(-1, -3, 0, 0), Rect.ScaleToEnclosedRect(new Rect(-1, -2, 0, 0), 1.5f));
        Assert.Equal(new Rect(1, 2, 2, 1), Rect.ScaleToEnclosedRect(new Rect(2, 4, 9, 8), 0.3f));
        TestScaleRectOverflowClamp((rect, x, y) => Rect.ScaleToEnclosedRect(rect, x, y));
    }
}
