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

    [Fact]
    private static void TestScaleToEnclosingRect()
    {
        Assert.Equal(new Rect(), Rect.ScaleToEnclosingRect(new Rect(), 5.0f));
        Assert.Equal(new Rect(5, 5, 5, 5), Rect.ScaleToEnclosingRect(new Rect(1, 1, 1, 1), 5.0f));
        Assert.Equal(new Rect(-5, -5, 0, 0), Rect.ScaleToEnclosingRect(new Rect(-1, -1, 0, 0), 5.0f));
        Assert.Equal(new Rect(5, -5, 0, 5), Rect.ScaleToEnclosingRect(new Rect(1, -1, 0, 1), 5.0f));
        Assert.Equal(new Rect(-5, 5, 5, 0), Rect.ScaleToEnclosingRect(new Rect(-1, 1, 1, 0), 5.0f));
        Assert.Equal(new Rect(1, 3, 5, 6), Rect.ScaleToEnclosingRect(new Rect(1, 2, 3, 4), 1.5f));
        Assert.Equal(new Rect(-2, -3, 0, 0), Rect.ScaleToEnclosingRect(new Rect(-1, -2, 0, 0), 1.5f));
        Assert.Equal(new Rect(0, 1, 4, 3), Rect.ScaleToEnclosingRect(new Rect(2, 4, 9, 8), 0.3f));
        TestScaleRectOverflowClamp((rect, x, y) => Rect.ScaleToEnclosingRect(rect, x, y));
    }

    [Fact]
    private static void TestScaleToRoundedRect()
    {
        Assert.Equal(new Rect(), Rect.ScaleToRoundedRect(new Rect(), 5.0f));
        Assert.Equal(new Rect(5, 5, 5, 5), Rect.ScaleToRoundedRect(new Rect(1, 1, 1, 1), 5.0f));
        Assert.Equal(new Rect(-5, -5, 0, 0), Rect.ScaleToRoundedRect(new Rect(-1, -1, 0, 0), 5.0f));
        Assert.Equal(new Rect(5, -5, 0, 5), Rect.ScaleToRoundedRect(new Rect(1, -1, 0, 1), 5.0f));
        Assert.Equal(new Rect(-5, 5, 5, 0), Rect.ScaleToRoundedRect(new Rect(-1, 1, 1, 0), 5.0f));
        Assert.Equal(new Rect(2, 3, 4, 6), Rect.ScaleToRoundedRect(new Rect(1, 2, 3, 4), 1.5f));
        Assert.Equal(new Rect(-2, -3, 0, 0), Rect.ScaleToRoundedRect(new Rect(-1, -2, 0, 0), 1.5f));
        Assert.Equal(new Rect(1, 1, 2, 3), Rect.ScaleToRoundedRect(new Rect(2, 4, 9, 8), 0.3f));
        TestScaleRectOverflowClamp((rect, x, y) => Rect.ScaleToRoundedRect(rect, x, y));
    }

    [Fact]
    private static void TestBoundingRect()
    {
        (Point a, Point b, Rect expected)[] int_tests =
        [
            // If point B dominates A, then A should be the origin.
            (new Point(4, 6), new Point(4, 6), new Rect(4, 6, 0, 0)),
            (new Point(4, 6), new Point(8, 6), new Rect(4, 6, 4, 0)),
            (new Point(4, 6), new Point(4, 9), new Rect(4, 6, 0, 3)),
            (new Point(4, 6), new Point(8, 9), new Rect(4, 6, 4, 3)),
            // If point A dominates B, then B should be the origin.
            (new Point(4, 6), new Point(4, 6), new Rect(4, 6, 0, 0)),
            (new Point(8, 6), new Point(4, 6), new Rect(4, 6, 4, 0)),
            (new Point(4, 9), new Point(4, 6), new Rect(4, 6, 0, 3)),
            (new Point(8, 9), new Point(4, 6), new Rect(4, 6, 4, 3)),
            // If neither point dominates, then the origin is a combination of the
            // two.
            (new Point(4, 6), new Point(6, 4), new Rect(4, 4, 2, 2)),
            (new Point(-4, -6), new Point(-6, -4), new Rect(-6, -6, 2, 2)),
            (new Point(-4, 6), new Point(6, -4), new Rect(-4, -4, 10, 10))
        ];

        foreach(var (a, b, expected) in int_tests)
        {
            Rect actual = Rect.BoundingRect(a, b);
            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    private static void TestOffset()
    {
        Rect i = new(1, 2, 3, 4);

        Assert.Equal(new Rect(2, 1, 3, 4), (i + new Vector2D(1, -1)));
        Assert.Equal(new Rect(2, 1, 3, 4), (new Vector2D(1, -1) + i));
        i += new Vector2D(1, -1);
        Assert.Equal(new Rect(2, 1, 3, 4), i);
        Assert.Equal(new Rect(1, 2, 3, 4), (i - new Vector2D(1, -1)));
        i -= new Vector2D(1, -1);
        Assert.Equal(new Rect(1, 2, 3, 4), i);

        i.Offset(2, -2);
        Assert.Equal(new Rect(3, 0, 3, 4), i);

        Assert.Equal(new Rect(kMaxInt - 2, kMaxInt - 2, 2, 2),
                  (new Rect(0, 0, kMaxInt - 2, kMaxInt - 2) +
                   new Vector2D(kMaxInt - 2, kMaxInt - 2)));
        Assert.Equal(new Rect(kMaxInt - 2, kMaxInt - 2, 2, 2),
                  (new Rect(0, 0, kMaxInt - 2, kMaxInt - 2) -
                   new Vector2D(2 - kMaxInt, 2 - kMaxInt)));
    }

    [Fact]
    private static void TestCorners()
    {
        Rect i = new(1, 2, 3, 4);
        Assert.Equal(new Point(1, 2), i.Origin);
        Assert.Equal(new Point(4, 2), i.TopRight);
        Assert.Equal(new Point(1, 6), i.BottomLeft);
        Assert.Equal(new Point(4, 6), i.BottomRight);
    }

    [Fact]
    private static void TestCenters()
    {
        Rect i = new(10, 20, 30, 40);
        Assert.Equal(new Point(10, 40), i.LeftCenter);
        Assert.Equal(new Point(25, 20), i.TopCenter);
        Assert.Equal(new Point(40, 40), i.RightCenter);
        Assert.Equal(new Point(25, 60), i.BottomCenter);
    }

    [Fact]
    private static void TestTranspose()
    {
        Rect i = new(10, 20, 30, 40);
        i.Transpose();
        Assert.Equal(new Rect(20, 10, 40, 30), i);
    }

    [Fact]
    private static void TestManhattanDistanceToPoint()
    {
        Rect i = new(1, 2, 3, 4);
        Assert.Equal(0, i.ManhattanDistanceToPoint(new Point(1, 2)));
        Assert.Equal(0, i.ManhattanDistanceToPoint(new Point(4, 6)));
        Assert.Equal(0, i.ManhattanDistanceToPoint(new Point(2, 4)));
        Assert.Equal(3, i.ManhattanDistanceToPoint(new Point(0, 0)));
        Assert.Equal(2, i.ManhattanDistanceToPoint(new Point(2, 0)));
        Assert.Equal(3, i.ManhattanDistanceToPoint(new Point(5, 0)));
        Assert.Equal(1, i.ManhattanDistanceToPoint(new Point(5, 4)));
        Assert.Equal(3, i.ManhattanDistanceToPoint(new Point(5, 8)));
        Assert.Equal(2, i.ManhattanDistanceToPoint(new Point(3, 8)));
        Assert.Equal(2, i.ManhattanDistanceToPoint(new Point(0, 7)));
        Assert.Equal(1, i.ManhattanDistanceToPoint(new Point(0, 3)));
    }

    [Fact]
    private static void TestManhattanInternalDistance()
    {
        Rect i = new(0, 0, 400, 400);
        Assert.Equal(0, i.ManhattanInternalDistance(new Rect(-1, 0, 2, 1)));
        Assert.Equal(1, i.ManhattanInternalDistance(new Rect(400, 0, 1, 400)));
        Assert.Equal(2, i.ManhattanInternalDistance(new Rect(-100, -100, 100, 100)));
        Assert.Equal(2, i.ManhattanInternalDistance(new Rect(-101, 100, 100, 100)));
        Assert.Equal(4, i.ManhattanInternalDistance(new Rect(-101, -101, 100, 100)));
        Assert.Equal(435, i.ManhattanInternalDistance(new Rect(630, 603, 100, 100)));
    }

    [Fact]
    private static void TestIntegerOverflow()
    {
        int limit = int.MaxValue;
        int min_limit = int.MinValue;
        int expected_thickness = 10;
        int large_number = limit - expected_thickness;

        Rect height_overflow = new(0, large_number, 100, 100);
        Assert.Equal(large_number, height_overflow.Y);
        Assert.Equal(expected_thickness, height_overflow.Height);

        Rect width_overflow = new(large_number, 0, 100, 100);
        Assert.Equal(large_number, width_overflow.X);
        Assert.Equal(expected_thickness, width_overflow.Width);

        Rect size_height_overflow = new(new Point(0, large_number), new Size(100, 100));
        Assert.Equal(large_number, size_height_overflow.Y);
        Assert.Equal(expected_thickness, size_height_overflow.Height);

        Rect size_width_overflow = new(new Point(large_number, 0), new Size(100, 100));
        Assert.Equal(large_number, size_width_overflow.X);
        Assert.Equal(expected_thickness, size_width_overflow.Width);

        Rect set_height_overflow = new(0, large_number, 100, 5);
        Assert.Equal(5, set_height_overflow.Height);
        set_height_overflow.Height = 100;
        Assert.Equal(expected_thickness, set_height_overflow.Height);

        Rect set_y_overflow = new(100, 100, 100, 100);
        Assert.Equal(100, set_y_overflow.Height);
        set_y_overflow.Y = large_number;
        Assert.Equal(expected_thickness, set_y_overflow.Height);

        Rect set_width_overflow = new(large_number, 0, 5, 100);
        Assert.Equal(5, set_width_overflow.Width);
        set_width_overflow.Width = 100;
        Assert.Equal(expected_thickness, set_width_overflow.Width);

        Rect set_x_overflow = new(100, 100, 100, 100);
        Assert.Equal(100, set_x_overflow.Width);
        set_x_overflow.X = large_number;
        Assert.Equal(expected_thickness, set_x_overflow.Width);

        Point large_offset = new(large_number, large_number);
        Size size = new(100, 100);
        Size expected_size = new(10, 10);

        Rect set_origin_overflow = new(100, 100, 100, 100);
        Assert.Equal(size, set_origin_overflow.Size);
        set_origin_overflow.Origin = large_offset;
        Assert.Equal(large_offset, set_origin_overflow.Origin);
        Assert.Equal(expected_size, set_origin_overflow.Size);

        Rect set_size_overflow = new(large_number, large_number, 5, 5);
        Assert.Equal(new Size(5, 5), set_size_overflow.Size);
        set_size_overflow.Size = size;
        Assert.Equal(large_offset, set_size_overflow.Origin);
        Assert.Equal(expected_size, set_size_overflow.Size);

        Rect set_rect_overflow = new();
        set_rect_overflow.SetRect(large_number, large_number, 100, 100);
        Assert.Equal(large_offset, set_rect_overflow.Origin);
        Assert.Equal(expected_size, set_rect_overflow.Size);

        // Insetting an empty rect, but the total inset (left + right) could overflow.
        Rect inset_overflow = new();
        inset_overflow.Inset(Insets.TLBR(large_number, large_number, 100, 100));
        Assert.Equal(large_offset, inset_overflow.Origin);
        Assert.Equal(new Size(), inset_overflow.Size);

        // Insetting where the total inset (width - left - right) could overflow.
        // Also, this insetting by the min limit in all directions cannot
        // represent Width without overflow, so that will also clamp.
        Rect inset_overflow2 = new();
        inset_overflow2.Inset(min_limit);
        Assert.Equal(inset_overflow2, new Rect(min_limit, min_limit, limit, limit));

        // Insetting where the width shouldn't change, but if the insets operations
        // clamped in the wrong order, e.g. ((width - left) - right) vs (width - (left
        // + right)) then this will not work properly.  This is the proper order,
        // as if left + right overflows, the width cannot be decreased by more than
        // max int anyway.  Additionally, if left + right underflows, it cannot be
        // increased by more then max int.
        Rect inset_overflow3 = new(0, 0, limit, limit);
        inset_overflow3.Inset(Insets.TLBR(-100, -100, 100, 100));
        Assert.Equal(inset_overflow3, new Rect(-100, -100, limit, limit));

        Rect inset_overflow4 = new(-1000, -1000, limit, limit);
        inset_overflow4.Inset(Insets.TLBR(100, 100, -100, -100));
        Assert.Equal(inset_overflow4, new Rect(-900, -900, limit, limit));

        Rect offset_overflow = new(0, 0, 100, 100);
        offset_overflow.Offset(large_number, large_number);
        Assert.Equal(large_offset, offset_overflow.Origin);
        Assert.Equal(expected_size, offset_overflow.Size);

        Rect operator_overflow = new(0, 0, 100, 100);
        operator_overflow += new Vector2D(large_number, large_number);
        Assert.Equal(large_offset, operator_overflow.Origin);
        Assert.Equal(expected_size, operator_overflow.Size);

        Rect origin_maxint = new(limit, limit, limit, limit);
        Assert.Equal(origin_maxint, new Rect(new Point(limit, limit), new Size()));

        // Expect a rect at the origin and a rect whose right/bottom is maxint
        // create a rect that extends from 0..maxint in both extents.
        {
            Rect origin_small = new(0, 0, 100, 100);
            Rect big_clamped = new(50, 50, limit, limit);
            Assert.Equal(big_clamped.Right, limit);

            Rect unioned = Rect.UnionRects(origin_small, big_clamped);
            Rect rect_limit = new(0, 0, limit, limit);
            Assert.Equal(unioned, rect_limit);
        }

        // Expect a rect that would overflow width (but not right) to be clamped
        // and to have maxint extents after unioning.
        {
            Rect small = new(-500, -400, 100, 100);
            Rect big = new(-400, -500, limit, limit);
            // Technically, this should be limit + 100 width, but will clamp to maxint.
            Assert.Equal(Rect.UnionRects(small, big), new Rect(-500, -500, limit, limit));
        }

        // Expect a rect that would overflow right *and* width to be clamped.
        {
            Rect clamped = new(500, 500, limit, limit);
            Rect positive_origin = new(100, 100, 500, 500);

            // Ideally, this should be (100, 100, limit + 400, limit + 400).
            // However, width overflows and would be clamped to limit, but right
            // overflows too and so will be clamped to limit - 100.
            Rect expected_rect = new(100, 100, limit - 100, limit - 100);
            Assert.Equal(Rect.UnionRects(clamped, positive_origin), expected_rect);
        }

        // Unioning a left=minint rect with a right=maxint rect.
        // We can't represent both ends of the spectrum in the same rect.
        // Make sure we keep the most useful area.
        {
            int part_limit = min_limit / 3;
            Rect left_minint = new(min_limit, min_limit, 1, 1);
            Rect right_maxint = new(limit - 1, limit - 1, limit, limit);
            Rect expected_rect = new(part_limit, part_limit, 2 * part_limit, 2 * part_limit);
            Rect result = Rect.UnionRects(left_minint, right_maxint);

            // The result should be maximally big.
            Assert.Equal(limit, result.Height);
            Assert.Equal(limit, result.Width);

            // The result should include the area near the origin.
            Assert.True(-part_limit > result.X);
            Assert.True(part_limit < result.Right);
            Assert.True(-part_limit > result.Y);
            Assert.True(part_limit < result.Bottom);

            // More succinctly, but harder to read in the results.
            Assert.True(Rect.UnionRects(left_minint, right_maxint).Contains(expected_rect));
        }
    }

    [Fact]
    private static void TestInset()
    {
        Rect r = new(10, 20, 30, 40);
        r.Inset(0);
        Assert.Equal(new Rect(10, 20, 30, 40), r);
        r.Inset(1);
        Assert.Equal(new Rect(11, 21, 28, 38), r);
        r.Inset(-1);
        Assert.Equal(new Rect(10, 20, 30, 40), r);

        r.Inset(Insets.VH(2, 1));
        Assert.Equal(new Rect(11, 22, 28, 36), r);
        r.Inset(Insets.VH(-2, -1));
        Assert.Equal(new Rect(10, 20, 30, 40), r);

        // The parameters are left, top, right, bottom.
        r.Inset(Insets.TLBR(2, 1, 4, 3));
        Assert.Equal(new Rect(11, 22, 26, 34), r);
        r.Inset(Insets.TLBR(-2, -1, -4, -3));
        Assert.Equal(new Rect(10, 20, 30, 40), r);

        r.Inset(Insets.TLBR(1, 2, 3, 4));
        Assert.Equal(new Rect(12, 21, 24, 36), r);
        r.Inset(Insets.TLBR(-1, -2, -3, -4));
        Assert.Equal(new Rect(10, 20, 30, 40), r);
    }

    [Fact]
    private static void TestOutset()
    {
        Rect r = new(10, 20, 30, 40);
        r.Outset(0);
        Assert.Equal(new Rect(10, 20, 30, 40), r);
        r.Outset(1);
        Assert.Equal(new Rect(9, 19, 32, 42), r);
        r.Outset(-1);
        Assert.Equal(new Rect(10, 20, 30, 40), r);

        r.Outset(Outsets.VH(2, 1));
        Assert.Equal(new Rect(9, 18, 32, 44), r);
        r.Outset(Outsets.VH(-2, -1));
        Assert.Equal(new Rect(10, 20, 30, 40), r);

        r.Outset(Outsets.TLBR(2, 1, 4, 3));
        Assert.Equal(new Rect(9, 18, 34, 46), r);
        r.Outset(Outsets.TLBR(-2, -1, -4, -3));
        Assert.Equal(new Rect(10, 20, 30, 40), r);
    }

    [Fact]
    private static void TestInsetOutsetClamped()
    {
        Rect r = new(10, 20, 30, 40);
        r.Inset(18);
        Assert.Equal(new Rect(28, 38, 0, 4), r);
        r.Inset(-18);
        Assert.Equal(new Rect(10, 20, 36, 40), r);

        r.Inset(Insets.VH(30, 15));
        Assert.Equal(new Rect(25, 50, 6, 0), r);
        r.Inset(Insets.VH(-30, -15));
        Assert.Equal(new Rect(10, 20, 36, 60), r);

        r.Inset(Insets.TLBR(30, 20, 50, 40));
        Assert.Equal(new Rect(30, 50, 0, 0), r);
        r.Inset(Insets.TLBR(-30, -20, -50, -40));
        Assert.Equal(new Rect(10, 20, 60, 80), r);

        r.Outset(kMaxInt);
        Assert.Equal(new Rect(10 - kMaxInt, 20 - kMaxInt, kMaxInt, kMaxInt), r);

        var outsets = new Outsets();
        outsets.SetTopBottom(kMaxInt, kMaxInt);
        r.Outset(outsets);
        Assert.Equal(new Rect(10 - kMaxInt, kMinInt, kMaxInt, kMaxInt), r);

        outsets = new Outsets();
        outsets.SetRight(kMaxInt);
        outsets.SetTop(kMaxInt);
        r.Outset(outsets);
        Assert.Equal(new Rect(10 - kMaxInt, kMinInt, kMaxInt, kMaxInt), r);

        outsets = new Outsets();
        outsets.SetLeftRight(kMaxInt, kMaxInt);
        r.Outset(outsets);
        Assert.Equal(new Rect(kMinInt, kMinInt, kMaxInt, kMaxInt), r);
    }

    [Fact]
    private static void TestSetByBounds()
    {
        Rect r = new();
        r.SetByBounds(1, 2, 30, 40);
        Assert.Equal(new Rect(1, 2, 29, 38), r);
        r.SetByBounds(30, 40, 1, 2);
        Assert.Equal(new Rect(30, 40, 0, 0), r);

        r.SetByBounds(0, 0, kMaxInt, kMaxInt);
        Assert.Equal(new Rect(0, 0, kMaxInt, kMaxInt), r);
        r.SetByBounds(-1, -1, kMaxInt, kMaxInt);
        Assert.Equal(new Rect(-1, -1, kMaxInt, kMaxInt), r);
        r.SetByBounds(1, 1, kMaxInt, kMaxInt);
        Assert.Equal(new Rect(1, 1, kMaxInt - 1, kMaxInt - 1), r);
        r.SetByBounds(kMinInt, kMinInt, 0, 0);
        Assert.Equal(new Rect(kMinInt + 1, kMinInt + 1, kMaxInt, kMaxInt), r);
        r.SetByBounds(kMinInt, kMinInt, 1, 1);
        Assert.Equal(new Rect(kMinInt + 2, kMinInt + 2, kMaxInt, kMaxInt), r);
        r.SetByBounds(kMinInt, kMinInt, -1, -1);
        Assert.Equal(new Rect(kMinInt, kMinInt, kMaxInt, kMaxInt), r);
        r.SetByBounds(kMinInt, kMinInt, kMaxInt, kMaxInt);
        Assert.Equal(new Rect(kMinInt / 2 - 1, kMinInt / 2 - 1, kMaxInt, kMaxInt), r);
    }

    [Fact]
    private static void TestMaximumCoveredRect()
    {
        // X aligned and intersect: unite.
        Assert.Equal(new Rect(10, 20, 30, 60),
                  Rect.MaximumCoveredRect(new Rect(10, 20, 30, 40), new Rect(10, 30, 30, 50)));
        // X aligned and adjacent: unite.
        Assert.Equal(new Rect(10, 20, 30, 90),
                  Rect.MaximumCoveredRect(new Rect(10, 20, 30, 40), new Rect(10, 60, 30, 50)));
        // X aligned and separate: choose the bigger one.
        Assert.Equal(new Rect(10, 61, 30, 50),
                  Rect.MaximumCoveredRect(new Rect(10, 20, 30, 40), new Rect(10, 61, 30, 50)));
        // Y aligned and intersect: unite.
        Assert.Equal(new Rect(10, 20, 60, 40),
                  Rect.MaximumCoveredRect(new Rect(10, 20, 30, 40), new Rect(30, 20, 40, 40)));
        // Y aligned and adjacent: unite.
        Assert.Equal(new Rect(10, 20, 70, 40),
                  Rect.MaximumCoveredRect(new Rect(10, 20, 30, 40), new Rect(40, 20, 40, 40)));
        // Y aligned and separate: choose the bigger one.
        Assert.Equal(new Rect(41, 20, 40, 40),
                  Rect.MaximumCoveredRect(new Rect(10, 20, 30, 40), new Rect(41, 20, 40, 40)));
        // Get the biggest expanded intersection.
        Assert.Equal(new Rect(0, 0, 9, 19),
                  Rect.MaximumCoveredRect(new Rect(0, 0, 10, 10), new Rect(0, 9, 9, 10)));
        Assert.Equal(new Rect(0, 0, 19, 9),
                  Rect.MaximumCoveredRect(new Rect(0, 0, 10, 10), new Rect(9, 0, 10, 9)));
        // Otherwise choose the bigger one.
        Assert.Equal(new Rect(20, 30, 40, 50),
                  Rect.MaximumCoveredRect(new Rect(10, 20, 30, 40), new Rect(20, 30, 40, 50)));
        Assert.Equal(new Rect(10, 20, 40, 50),
                  Rect.MaximumCoveredRect(new Rect(10, 20, 40, 50), new Rect(20, 30, 30, 40)));
        Assert.Equal(new Rect(10, 20, 40, 50),
                  Rect.MaximumCoveredRect(new Rect(10, 20, 40, 50), new Rect(20, 30, 40, 50)));
    }
}
