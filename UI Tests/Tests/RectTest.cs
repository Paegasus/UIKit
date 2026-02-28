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
}
