using Xunit;

using UI.GFX.Geometry;
using System.Diagnostics;
using SkiaSharp;

namespace UI.Tests;

public static class RRectFTest
{
    [Fact]
    private static void TestIsEmpty()
    {
        Assert.True(new RRectF().IsEmpty());
        Assert.True(new RRectF(0, 0, 0, 0, 0).IsEmpty());
        Assert.True(new RRectF(0, 0, 10, 0, 0).IsEmpty());
        Assert.True(new RRectF(0, 0, 0, 10, 0).IsEmpty());
        Assert.True(new RRectF(0, 0, 0, 10, 10).IsEmpty());
        Assert.False(new RRectF(0, 0, 10, 10, 0).IsEmpty());
    }

    [Fact]
    private static void TestEquals()
    {
        Assert.Equal(new RRectF(0, 0, 0, 0, 0, 0), new RRectF(0, 0, 0, 0, 0, 0));
        Assert.Equal(new RRectF(1, 2, 3, 4, 5, 6), new RRectF(1, 2, 3, 4, 5, 6));
        Assert.Equal(new RRectF(1, 2, 3, 4, 5, 5), new RRectF(1, 2, 3, 4, 5));
        Assert.Equal(new RRectF(0, 0, 2, 3, 0, 0), new RRectF(0, 0, 2, 3, 0, 1));
        Assert.Equal(new RRectF(0, 0, 2, 3, 0, 0), new RRectF(0, 0, 2, 3, 1, 0));
        Assert.Equal(new RRectF(1, 2, 3, 0, 5, 6), new RRectF(0, 0, 0, 0, 0, 0));
        Assert.Equal(new RRectF(0, 0, 0, 0, 5, 6), new RRectF(0, 0, 0, 0, 0, 0));

        Assert.NotEqual(new RRectF(10, 20, 30, 40, 7, 8), new RRectF(1, 20, 30, 40, 7, 8));
        Assert.NotEqual(new RRectF(10, 20, 30, 40, 7, 8), new RRectF(10, 2, 30, 40, 7, 8));
        Assert.NotEqual(new RRectF(10, 20, 30, 40, 7, 8), new RRectF(10, 20, 3, 40, 7, 8));
        Assert.NotEqual(new RRectF(10, 20, 30, 40, 7, 8), new RRectF(10, 20, 30, 4, 7, 8));
        Assert.NotEqual(new RRectF(10, 20, 30, 40, 7, 8), new RRectF(10, 20, 30, 40, 5, 8));
        Assert.NotEqual(new RRectF(10, 20, 30, 40, 7, 8), new RRectF(10, 20, 30, 40, 7, 6));
    }

    [Fact]
    private static void TestPlusMinusOffset()
    {
        RRectF a = new(40, 50, 60, 70, 5);
        Vector2D offset = new(23, 34);
        RRectF correct = new(63, 84, 60, 70, 5);
        RRectF b = a + offset;
        Assert.Equal(b, correct);
        b = a;
        b.Offset(offset);
        Assert.Equal(b, correct);

        correct = new RRectF(17, 16, 60, 70, 5);
        b = a - offset;
        Assert.Equal(b, correct);
        b = a;
        b.Offset(-offset);
        Assert.Equal(b, correct);
    }

    [Fact]
    private static void TestRRectTypes()
    {
        RRectF a = new(40, 50, 0, 70, 0);
        Assert.Equal(RRectF.RoundRectType.kEmpty, a.GetRoundRectType());
        Assert.True(a.IsEmpty());
        a = new RRectF(40, 50, 60, 70, 0);
        Assert.Equal(RRectF.RoundRectType.kRect, a.GetRoundRectType());
        a = new RRectF(40, 50, 60, 70, 5);
        Assert.Equal(RRectF.RoundRectType.kSingle, a.GetRoundRectType());
        a = new RRectF(40, 50, 60, 70, 5, 5);
        Assert.Equal(RRectF.RoundRectType.kSingle, a.GetRoundRectType());
        a = new RRectF(40, 50, 60, 60, 30, 30);
        Assert.Equal(RRectF.RoundRectType.kSingle, a.GetRoundRectType());
        a = new RRectF(40, 50, 60, 70, 6, 3);
        Assert.Equal(RRectF.RoundRectType.kSimple, a.GetRoundRectType());
        a = new RRectF(40, 50, 60, 70, 30, 3);
        Assert.Equal(RRectF.RoundRectType.kSimple, a.GetRoundRectType());
        a = new RRectF(40, 50, 60, 70, 30, 35);
        Assert.Equal(RRectF.RoundRectType.kOval, a.GetRoundRectType());

        a.SetCornerRadii(RRectF.RoundRectCorner.kLowerRight, new Vector2DF(7, 8));
        Assert.Equal(RRectF.RoundRectType.kComplex, a.GetRoundRectType());
        
        // When one radius is larger than half its dimension,
        // both radii are scaled down proportionately.
        a = new RRectF(40, 50, 60, 70, 30, 70);
        Assert.Equal(RRectF.RoundRectType.kSimple, a.GetRoundRectType());
        Assert.Equal(a, new RRectF(40, 50, 60, 70, 15, 35));
        // If they stay equal to half the radius, it stays oval.
        a = new RRectF(40, 50, 60, 70, 120, 140);
        Assert.Equal(RRectF.RoundRectType.kOval, a.GetRoundRectType());
    }

    private static void CheckRadii(RRectF val,
                                   float ulx,
                                   float uly,
                                   float urx,
                                   float ury,
                                   float lrx,
                                   float lry,
                                   float llx,
                                   float lly)
    {
        Assert.Equal(val.GetCornerRadii(RRectF.RoundRectCorner.kUpperLeft), new Vector2DF(ulx, uly));
        Assert.Equal(val.GetCornerRadii(RRectF.RoundRectCorner.kUpperRight), new Vector2DF(urx, ury));
        Assert.Equal(val.GetCornerRadii(RRectF.RoundRectCorner.kLowerRight), new Vector2DF(lrx, lry));
        Assert.Equal(val.GetCornerRadii(RRectF.RoundRectCorner.kLowerLeft), new Vector2DF(llx, lly));
    }

    [Fact]
    private static void TestRRectRadii()
    {
        RRectF a = new(40, 50, 60, 70, 0);
        CheckRadii(a, 0, 0, 0, 0, 0, 0, 0, 0);

        a.SetCornerRadii(RRectF.RoundRectCorner.kUpperLeft, 1, 2);
        CheckRadii(a, 1, 2, 0, 0, 0, 0, 0, 0);

        a.SetCornerRadii(RRectF.RoundRectCorner.kUpperRight, 3, 4);
        CheckRadii(a, 1, 2, 3, 4, 0, 0, 0, 0);

        a.SetCornerRadii(RRectF.RoundRectCorner.kLowerRight, 5, 6);
        CheckRadii(a, 1, 2, 3, 4, 5, 6, 0, 0);

        a.SetCornerRadii(RRectF.RoundRectCorner.kLowerLeft, 7, 8);
        CheckRadii(a, 1, 2, 3, 4, 5, 6, 7, 8);

        RRectF b = new(40, 50, 60, 70, 1, 2, 3, 4, 5, 6, 7, 8);
        Assert.Equal(a, b);
    }

    [Fact]
    private static void TestFromRectF()
    {
        // Check that explicit conversion from float rect works.
        RectF a = new(40, 50, 60, 70);
        RRectF b = new(40, 50, 60, 70, 0);
        RRectF c = new RRectF(a);
        Assert.Equal(b, c);
    }

    [Fact]
    private static void TestFromSkRRect()
    {
        // Check that explicit conversion from SkRRect works.
        SKRoundRect a = new(SKRect.Create(40, 50, 60, 70), 15, 25);
        RRectF b = new(40, 50, 60, 70, 15, 25);
        RRectF c = new(a);
        Assert.Equal(b, c);

        // Try with single radius constructor.
        a = new(SKRect.Create(40, 50, 60, 70), 15, 15);
        b = new RRectF(40, 50, 60, 70, 15);
        c = new RRectF(a);
        Assert.Equal(b, c);
    }

    [Fact]
    private static void TestFromRoundedCornersF()
    {
        RectF kRect = new(50.0f, 40.0f);
        RoundedCornersF kCorners = new(1.5f, 2.5f, 3.5f, 4.5f);
        RRectF rrect_f = new(kRect, kCorners);

        var upper_left = rrect_f.GetCornerRadii(RRectF.RoundRectCorner.kUpperLeft);
        Assert.Equal(kCorners.UpperLeft, upper_left.X);
        Assert.Equal(kCorners.UpperLeft, upper_left.Y);
        var upper_right = rrect_f.GetCornerRadii(RRectF.RoundRectCorner.kUpperRight);
        Assert.Equal(kCorners.UpperRight, upper_right.X);
        Assert.Equal(kCorners.UpperRight, upper_right.Y);
        var lower_right = rrect_f.GetCornerRadii(RRectF.RoundRectCorner.kLowerRight);
        Assert.Equal(kCorners.LowerRight, lower_right.X);
        Assert.Equal(kCorners.LowerRight, lower_right.Y);
        var lower_left = rrect_f.GetCornerRadii(RRectF.RoundRectCorner.kLowerLeft);
        Assert.Equal(kCorners.LowerLeft, lower_left.X);
        Assert.Equal(kCorners.LowerLeft, lower_left.Y);
    }

    [Fact]
    private static void TestToString()
    {
        RRectF a = new(40, 50, 60, 70, 0);

        Assert.Equal("40.000,50.000 60.000x70.000, rectangular", a.ToString());

        a = new RRectF(40, 50, 60, 70, 15);
        Assert.Equal("40.000,50.000 60.000x70.000, radius 15.000", a.ToString());

        a = new RRectF(40, 50, 60, 70, 15, 25);
        Assert.Equal("40.000,50.000 60.000x70.000, x_rad 15.000, y_rad 25.000", a.ToString());

        a.SetCornerRadii(RRectF.RoundRectCorner.kLowerRight, new Vector2DF(7, 8));
        Assert.Equal("40.000,50.000 60.000x70.000, [15.000 25.000] [15.000 25.000] [7.000 8.000] [15.000 25.000]", a.ToString());
    }

    [Fact]
    private static void TestSizes()
    {
        RRectF a = new(40, 50, 60, 70, 5, 6);
        Assert.Equal(40, a.rect().X);
        Assert.Equal(50, a.rect().Y);
        Assert.Equal(60, a.rect().Width);
        Assert.Equal(70, a.rect().Height);
        Assert.Equal(5, a.GetSimpleRadii().X);
        Assert.Equal(6, a.GetSimpleRadii().Y);
        a = new RRectF(40, 50, 60, 70, 5, 5);
        Assert.Equal(5, a.GetSimpleRadius());
        a.Clear();
        Assert.True(a.IsEmpty());
        // Make sure ovals can still get simple radii
        a = new RRectF(40, 50, 60, 70, 30, 35);
        Assert.Equal(RRectF.RoundRectType.kOval, a.GetRoundRectType());
        Assert.Equal(30, a.GetSimpleRadii().X);
        Assert.Equal(35, a.GetSimpleRadii().Y);
    }

    [Fact]
    private static void TestContains()
    {
        RRectF a = new(40, 50, 60, 70, 5, 6);
        RectF b = new(50, 60, 5, 6);
        
        Assert.True(a.Contains(b));
        b = new RectF(40, 50, 5, 6);  // Right on the border
        Assert.False(a.Contains(b)); // <- FAILS
        b = new RectF(95, 114, 5, 6);  // Right on the border
        Assert.False(a.Contains(b));
        b = new RectF(40, 50, 60, 70);
        Assert.False(a.Contains(b));
    }

    [Fact]
    private static void TestHasRoundedCorners()
    {
        RRectF a = new();
        Assert.False(a.HasRoundedCorners());
        a = new RRectF(new RectF(10, 10));
        Assert.False(a.HasRoundedCorners());
        a = new RRectF(new RectF(), new RoundedCornersF(1.0f));
        Assert.False(a.HasRoundedCorners());
        a = new RRectF(new RectF(10, 10), new RoundedCornersF(1.0f));
        Assert.True(a.HasRoundedCorners());
        a = new RRectF(new RectF(10, 10), new RoundedCornersF(1, 0, 0, 0));
        Assert.True(a.HasRoundedCorners());
    }

    [Fact]
    private static void TestScale()
    {
        // source
        (float x1, float y1, float w1, float h1, float x_rad1, float y_rad1, float x_scale, float y_scale,
        // target
        float x2, float y2, float w2, float h2, float x_rad2, float y_rad2)[] tests =
        [
            (3.0f, 4.0f, 5.0f, 6.0f, 0.0f, 0.0f, 1.5f, 1.5f, 4.5f, 6.0f, 7.5f, 9.0f, 0.0f, 0.0f),
			(3.0f, 4.0f, 5.0f, 6.0f, 1.0f, 1.0f, 1.5f, 1.5f, 4.5f, 6.0f, 7.5f, 9.0f, 1.5f, 1.5f),
			(3.0f, 4.0f, 5.0f, 6.0f, 0.0f, 0.0f, 1.5f, 3.0f, 4.5f, 12.0f, 7.5f, 18.0f, 0.0f, 0.0f),
			(3.0f, 4.0f, 5.0f, 6.0f, 1.0f, 1.0f, 1.5f, 3.0f, 4.5f, 12.0f, 7.5f, 18.0f, 1.5f, 3.0f),
			(3.0f, 4.0f, 0.0f, 6.0f, 1.0f, 1.0f, 1.5f, 1.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f),
			(3.0f, 4.0f, 5.0f, 6.0f, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f),
			(3.0f, 4.0f, 5.0f, 6.0f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f),
			(3.0f, 4.0f, 5.0f, 6.0f, 1.0f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f),
			(3.0f, 4.0f, 5.0f, 6.0f, 1.0f, 1.0f, float.MaxValue, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f),
			(3.0f, 4.0f, 5.0f, 6.0f, 1.0f, 1.0f, 1.0f, float.MaxValue, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f),
			(3.0f, 4.0f, 5.0f, 6.0f, 1.0f, 1.0f, float.MaxValue, float.MaxValue, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f),
			(3.0f, 4.0f, 5.0f, 6.0f, 1.0f, 1.0f, float.NaN, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f),
			(3.0f, 4.0f, 5.0f, 6.0f, 1.0f, 1.0f, 1.0f, float.NaN, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f),
			(3.0f, 4.0f, 5.0f, 6.0f, 1.0f, 1.0f, float.NaN, float.NaN, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f)
        ];

        foreach (var (x1, y1, w1, h1, x_rad1, y_rad1, x_scale, y_scale, x2, y2, w2, h2, x_rad2, y_rad2) in tests)
        {
            RRectF r1 = new(x1, y1, w1, h1, x_rad1, y_rad1);
            RRectF r2 = new(x2, y2, w2, h2, x_rad2, y_rad2);

            r1.Scale(x_scale, y_scale);
            Assert.True(r1.GetRoundRectType() <= RRectF.RoundRectType.kSimple);
            Assert.Equal(r1.Rect().X, r2.Rect().X);
            Assert.Equal(r1.Rect().Y, r2.Rect().Y);
            Assert.Equal(r1.Rect().Width, r2.Rect().Width);
            Assert.Equal(r1.Rect().Height, r2.Rect().Height);
            Assert.Equal(r1.GetSimpleRadii(), r2.GetSimpleRadii());
        }
    }

    [Fact]
    private static void TestInsetOutset()
    {
        RRectF a = new(40, 50, 60, 70, 5);
        RRectF b = a;
        b.Inset(3);
        Assert.Equal(b, new RRectF(43, 53, 54, 64, 2));
        b = a;
        b.Outset(3);
        Assert.Equal(b, new RRectF(37, 47, 66, 76, 8));
    }

    // The following tests (started with "Build*") are for RRectFBuilder. All
    // different tests are to make sure that existing RRectF definitions can be
    // implemented with RRectFBuilder.

    [Fact]
    private static void TestBuildFromRectF()
    {
        RectF a = new();
        RRectF b = new(a);
        RRectF c = new RRectFBuilder().SetRect(a).Build();
        Assert.Equal(b, c);

        a = new RectF(60, 70);
        b = new RRectF(a);
        c = new RRectFBuilder().SetRect(a).Build();
        Assert.Equal(b, c);

        a = new RectF(40, 50, 60, 70);
        b = new RRectF(a);
        c = new RRectFBuilder().SetRect(a).Build();
        Assert.Equal(b, c);
    }

    [Fact]
    private static void TestBuildFromRadius()
    {
        RRectF a = new(40, 50, 60, 70, 15);
        RRectF b = new RRectFBuilder()
                       .SetOrigin(40, 50)
                       .SetSize(60, 70)
                       .SetRadius(15)
                       .Build();
        Assert.Equal(a, b);

        a = new RRectF(40, 50, 60, 70, 15, 25);
        b = new RRectFBuilder()
                .SetOrigin(40, 50)
                .SetSize(60, 70)
                .SetRadius(15, 25)
                .Build();
        Assert.Equal(a, b);

        PointF p = new(40, 50);
        SizeF s = new(60, 70);
        b = new RRectFBuilder().SetOrigin(p).SetSize(s).SetRadius(15, 25).Build();
        Assert.Equal(a, b);
    }

    [Fact]
    private static void TestBuildFromRectFWithRadius()
    {
        RectF a = new(40, 50, 60, 70);
        RRectF b = new(a, 15);
        RRectF c = new RRectFBuilder().SetRect(a).SetRadius(15).Build();
        Assert.Equal(b, c);

        b = new RRectF(a, 15, 25);
        c = new RRectFBuilder().SetRect(a).SetRadius(15, 25).Build();
        Assert.Equal(b, c);
    }

    [Fact]
    private static void TestBuildFromCorners()
    {
        RRectF a = new(40, 50, 60, 70, 1, 2, 3, 4, 5, 6, 7, 8);
        RRectF b = new RRectFBuilder()
                 .SetOrigin(40, 50)
                 .SetSize(60, 70)
                 .SetUpperLeft(1, 2)
                 .SetUpperRight(3, 4)
                 .SetLowerRight(5, 6)
                 .SetLowerLeft(7, 8)
                 .Build();
        Assert.Equal(a, b);
    }

    [Fact]
    private static void TestBuildFromRectFWithCorners()
    {
        RectF a = new(40, 50, 60, 70);
        RRectF b = new(a, 1, 2, 3, 4, 5, 6, 7, 8);
        RRectF c = new RRectFBuilder()
                       .SetRect(a)
                       .SetUpperLeft(1, 2)
                       .SetUpperRight(3, 4)
                       .SetLowerRight(5, 6)
                       .SetLowerLeft(7, 8)
                       .Build();
        Assert.Equal(b, c);
    }

    [Fact]
    private static void TestBuildFromRoundedCornersF()
    {
        RectF a = new(40, 50, 60, 70);
        RoundedCornersF corners = new(1.5f, 2.5f, 3.5f, 4.5f);
        RRectF b = new(a, corners);
        RRectF c = new RRectFBuilder().SetRect(a).SetCorners(corners).Build();
        Assert.Equal(b, c);
    }

    // In the following tests(*CornersHigherThanSize), we test whether the corner
    // radii gets truncated in case of being greater than the width/height.

    [Fact]
    private static void TestBuildFromCornersHigherThanSize()
    {
        RRectF a = new(0, 0, 20, 10, 12, 2, 8, 4, 14, 6, 6, 8);
        RRectF b = new RRectFBuilder()
                 .SetOrigin(0, 0)
                 .SetSize(20, 10)
                 .SetUpperLeft(48, 8)
                 .SetUpperRight(32, 16)
                 .SetLowerRight(56, 24)
                 .SetLowerLeft(24, 32)
                 .Build();
        Assert.Equal(a, b);
    }
}
