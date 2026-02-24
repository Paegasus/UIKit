using Xunit;

using UI.GFX.Geometry;

using static UI.GFX.Geometry.PointConversions;

namespace UI.Tests;

public static class LineFTest
{
    [Fact]
    private static void TestNormal()
    {
        LineF line = new()
        {
            p1 = new (10f, 10f),
            p2 = new (100f, 10f)
        };

        Assert.Equal(line.Normal(), new Vector2DF(0, 90));

        line = new()
        {
            p1 = new (10f, 10f),
            p2 = new (10f, 100f)
        };

        Assert.Equal(line.Normal(), new Vector2DF(-90, 0));

        line = new()
        {
            p1 = new (20f, 20f),
            p2 = new (100f, 100f)
        };

        Assert.Equal(line.Normal(), new Vector2DF(-80, 80));

        line = new()
        {
            p1 = new (5f, 5f),
            p2 = new (5f, 5f)
        };

        Assert.Equal(line.Normal(), new Vector2DF(0, 0));

        line = new()
        {
            p1 = new (0f, -15),
            p2 = new (15f, -10f)
        };

        Assert.Equal(line.Normal(), new Vector2DF(-5, 15));
    }

    [Fact]
    private static void TestIntersection()
    {
        LineF line1 = new()
        {
            p1 = new (10f, 10f),
            p2 = new (100f, 10f)
        };

        LineF line2 = new()
        {
            p1 = new (50f, 0f),
            p2 = new (50f, 100f)
        };

        Assert.Equal(line1.IntersectionWith(line2), new PointF(50, 10));

        line2 = new()
        {
            p1 = new (50f, 20f),
            p2 = new (60f, 20f)
        };

        Assert.Null(line1.IntersectionWith(line2));

        line1 = new()
        {
            p1 = new (10f, 10f),
            p2 = new (10f, 10f)
        };

        line2 = new()
        {
            p1 = new (50f, 30f),
            p2 = new (60f, 20f)
        };

        Assert.Null(line1.IntersectionWith(line2));

        line1 = new()
        {
            p1 = new (0f, 0f),
            p2 = new (20f, 20f)
        };

        line2 = new()
        {
            p1 = new (0f, 20f),
            p2 = new (20f, 0f)
        };

        Assert.Equal(line1.IntersectionWith(line2), new PointF(10, 10));

        line1 = new()
        {
            p1 = new (0f, -10f),
            p2 = new (-20f, 20f)
        };

        line2 = new()
        {
            p1 = new (-10f, 20f),
            p2 = new (-100f, -100f)
        };
        
        Assert.Equal(ToRoundedPoint(line1.IntersectionWith(line2)!.Value), ToRoundedPoint(new PointF(-15.2941f, 12.9412f)));
    }
}
