using Xunit;

using UI.GFX.Geometry;
using UI.Extensions;

using static UI.GFX.Geometry.PointConversions;

namespace UI.Tests;

public static class PointFTest
{
    [Fact]
    private static void TestPointToPointF()
    {
        // Check that explicit conversion from integer to float compiles.
        Point a = new(10, 20);
        PointF b = new(a);

        Assert.Equal((float)a.X, b.X);
        Assert.Equal((float)a.Y, b.Y);
    }

    [Fact]
    private static void TestIsOrigin()
    {
        Assert.False(new PointF(0.1f, 0).IsOrigin());
        Assert.False(new PointF(0, 0.1f).IsOrigin());
        Assert.False(new PointF(0.1f, 2).IsOrigin());
        Assert.False(new PointF(-0.1f, 0).IsOrigin());
        Assert.False(new PointF(0, -0.1f).IsOrigin());
        Assert.False(new PointF(-0.1f, -2).IsOrigin());
        Assert.True(new PointF(0, 0).IsOrigin());
    }

    [Fact]
    private static void TestToRoundedPoint()
    {
        Assert.Equal(new Point(0, 0), ToRoundedPoint(new PointF(0, 0)));
        Assert.Equal(new Point(0, 0), ToRoundedPoint(new PointF(0.0001f, 0.0001f)));
        Assert.Equal(new Point(0, 0), ToRoundedPoint(new PointF(0.4999f, 0.4999f)));
        Assert.Equal(new Point(1, 1), ToRoundedPoint(new PointF(0.5f, 0.5f)));
        Assert.Equal(new Point(1, 1), ToRoundedPoint(new PointF(0.9999f, 0.9999f)));

        Assert.Equal(new Point(10, 10), ToRoundedPoint(new PointF(10, 10)));
        Assert.Equal(new Point(10, 10), ToRoundedPoint(new PointF(10.0001f, 10.0001f)));
        Assert.Equal(new Point(10, 10), ToRoundedPoint(new PointF(10.4999f, 10.4999f)));
        Assert.Equal(new Point(11, 11), ToRoundedPoint(new PointF(10.5f, 10.5f)));
        Assert.Equal(new Point(11, 11), ToRoundedPoint(new PointF(10.9999f, 10.9999f)));

        Assert.Equal(new Point(-10, -10), ToRoundedPoint(new PointF(-10, -10)));
        Assert.Equal(new Point(-10, -10), ToRoundedPoint(new PointF(-10.0001f, -10.0001f)));
        Assert.Equal(new Point(-10, -10), ToRoundedPoint(new PointF(-10.4999f, -10.4999f)));
        Assert.Equal(new Point(-11, -11), ToRoundedPoint(new PointF(-10.5f, -10.5f)));
        Assert.Equal(new Point(-11, -11), ToRoundedPoint(new PointF(-10.9999f, -10.9999f)));
    }

    [Fact]
    private static void TestScale()
    {
        Assert.Equal(new PointF(2.0f, -2.0f), PointF.ScalePoint(new PointF(1.0f, -1.0f), 2.0f));
        Assert.Equal(new PointF(2.0f, -2.0f), PointF.ScalePoint(new PointF(1.0f, -1.0f), 2.0f, 2.0f));

        PointF zero = new();
        PointF one = new(1.0f, -1.0f);

        zero.Scale(2.0f);
        zero.Scale(3.0f, 1.5f);

        one.Scale(2.0f);
        one.Scale(3.0f, 1.5f);

        Assert.Equal(new PointF(), zero);
        Assert.Equal(new PointF(6.0f, -3.0f), one);
    }

    [Fact]
    private static void TestSetToMinMax()
    {
        PointF a;

        a = new PointF(3.5f, 5.5f);
        Assert.Equal(new PointF(3.5f, 5.5f), a);
        a.SetToMax(new PointF(2.5f, 4.5f));
        Assert.Equal(new PointF(3.5f, 5.5f), a);
        a.SetToMax(new PointF(3.5f, 5.5f));
        Assert.Equal(new PointF(3.5f, 5.5f), a);
        a.SetToMax(new PointF(4.5f, 2.5f));
        Assert.Equal(new PointF(4.5f, 5.5f), a);
        a.SetToMax(new PointF(8.5f, 10.5f));
        Assert.Equal(new PointF(8.5f, 10.5f), a);

        a.SetToMin(new PointF(9.5f, 11.5f));
        Assert.Equal(new PointF(8.5f, 10.5f), a);
        a.SetToMin(new PointF(8.5f, 10.5f));
        Assert.Equal(new PointF(8.5f, 10.5f), a);
        a.SetToMin(new PointF(11.5f, 9.5f));
        Assert.Equal(new PointF(8.5f, 9.5f), a);
        a.SetToMin(new PointF(7.5f, 11.5f));
        Assert.Equal(new PointF(7.5f, 9.5f), a);
        a.SetToMin(new PointF(3.5f, 5.5f));
        Assert.Equal(new PointF(3.5f, 5.5f), a);
    }

    [Fact]
    private static void TestIsWithinDistance()
    {
        PointF pt = new(10.0f, 10.0f);
        Assert.True(pt.IsWithinDistance(new PointF(10.0f, 10.0f), 0.0000000000001f));
        Assert.False(pt.IsWithinDistance(new PointF(8.0f, 8.0f), 1.0f));

        pt = new PointF(-10.0f, -10.0f);
        Assert.False(
            pt.IsWithinDistance(new PointF(10.0f, 10.0f), /*allowed_distance=*/10.0f));
        Assert.True(pt.IsWithinDistance(new PointF(-9.9988f, -10.0013f), 0.0017689f));

        pt = new PointF(float.MaxValue, float.MaxValue);
        Assert.False(pt.IsWithinDistance(new PointF(float.MinNormal, float.MinNormal), 100.0f));
    }

    [Fact]
    private static void TestTranspose()
    {
        PointF p = new(-1.5f, 2.5f);
        Assert.Equal(new PointF(2.5f, -1.5f), PointF.TransposePoint(p));
        p.Transpose();
        Assert.Equal(new PointF(2.5f, -1.5f), p);
    }

    [Fact]
    private static void TestToString()
    {
        Assert.Equal("1,2", new PointF(1, 2).ToString());
        Assert.Equal("1.03125,2.5", new PointF(1.03125f, 2.5f).ToString());
    }
}
