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

    [Fact]
    private static void TestOffsetFromPoint()
    {
        Point a = new(1, 5);
        Point b = new(-20, 8);
        Assert.Equal(new Vector2D(-20 - 1, 8 - 5), (b - a));
    }

    [Fact]
    private static void TestSetToMinMax()
    {
        Point a;

        a = new Point(3, 5);
        Assert.Equal(new Point(3, 5), a);
        a.SetToMax(new Point(2, 4));
        Assert.Equal(new Point(3, 5), a);
        a.SetToMax(new Point(3, 5));
        Assert.Equal(new Point(3, 5), a);
        a.SetToMax(new Point(4, 2));
        Assert.Equal(new Point(4, 5), a);
        a.SetToMax(new Point(8, 10));
        Assert.Equal(new Point(8, 10), a);

        a.SetToMin(new Point(9, 11));
        Assert.Equal(new Point(8, 10), a);
        a.SetToMin(new Point(8, 10));
        Assert.Equal(new Point(8, 10), a);
        a.SetToMin(new Point(11, 9));
        Assert.Equal(new Point(8, 9), a);
        a.SetToMin(new Point(7, 11));
        Assert.Equal(new Point(7, 9), a);
        a.SetToMin(new Point(3, 5));
        Assert.Equal(new Point(3, 5), a);
    }

    [Fact]
    private static void TestOffset()
    {
        Point test = new(3, 4);
        test.Offset(5, -8);
        Assert.Equal(test, new Point(8, -4));
    }

    [Fact]
    private static void TestVectorMath()
    {
        Point test = new(3, 4);
        test += new Vector2D(5, -8);
        Assert.Equal(test, new Point(8, -4));

        Point test2 = new Point(3, 4);
        test2 -= new Vector2D(5, -8);
        Assert.Equal(test2, new Point(-2, 12));
    }

    [Fact]
    private static void TestIntegerOverflow()
    {
        int int_max = int.MaxValue;
        int int_min = int.MinValue;

        Point max_point = new(int_max, int_max);
        Point min_point = new(int_min, int_min);
        Point test;

        test = new Point();
        test.Offset(int_max, int_max);
        Assert.Equal(test, max_point);

        test = new Point();
        test.Offset(int_min, int_min);
        Assert.Equal(test, min_point);

        test = new Point(10, 20);
        test.Offset(int_max, int_max);
        Assert.Equal(test, max_point);

        test = new Point(-10, -20);
        test.Offset(int_min, int_min);
        Assert.Equal(test, min_point);

        test = new Point();
        test += new Vector2D(int_max, int_max);
        Assert.Equal(test, max_point);

        test = new Point();
        test += new Vector2D(int_min, int_min);
        Assert.Equal(test, min_point);

        test = new Point(10, 20);
        test += new Vector2D(int_max, int_max);
        Assert.Equal(test, max_point);

        test = new Point(-10, -20);
        test += new Vector2D(int_min, int_min);
        Assert.Equal(test, min_point);

        test = new Point();
        test -= new Vector2D(int_max, int_max);
        Assert.Equal(test, new Point(-int_max, -int_max));

        test = new Point();
        test -= new Vector2D(int_min, int_min);
        Assert.Equal(test, max_point);

        test = new Point(10, 20);
        test -= new Vector2D(int_min, int_min);
        Assert.Equal(test, max_point);

        test = new Point(-10, -20);
        test -= new Vector2D(int_max, int_max);
        Assert.Equal(test, min_point);
    }

    [Fact]
    private static void TestTranspose()
    {
        Point p = new(1, -2);
        Assert.Equal(new Point(-2, 1), Point.TransposePoint(p));
        p.Transpose();
        Assert.Equal(new Point(-2, 1), p);
    }
}
