using Xunit;

using UI.GFX.Geometry;
using SkiaSharp;

using static UI.GFX.Geometry.PointConversions;
using static UI.GFX.Geometry.SkiaConversions;

namespace UI.Tests;

public static class SkiaConversionsTest
{
    [Fact]
    private static void TestSkiaRectConversions()
    {
        Rect isrc = new(10, 20, 30, 40);
        RectF fsrc = new(10.5f, 20.5f, 30.5f, 40.5f);

        SKRectI skirect = RectToSkIRect(isrc);
        Assert.Equal(isrc.ToString(), SkIRectToRect(skirect).ToString());

        SKRect skrect = RectToSkRect(isrc);
        Assert.Equal(new RectF(isrc).ToString(), SkRectToRectF(skrect).ToString());

        skrect = RectFToSkRect(fsrc);
        Assert.Equal(fsrc.ToString(), SkRectToRectF(skrect).ToString());
    }

    [Fact]
    private static void TestRectToSkRectAccuracy()
    {
        // For a Rect with large negative x/y and large with/height, but small right/bottom,
        // we expect the converted SkRect has accurate right/bottom,
        // to make sure the right/bottom edge, which is likely to be visible, to be rendered correctly.
        Rect r = new();
        for (int i = 0; i < 50; i++)
        {
            r.SetByBounds(-30000000, -28000000, i, i + 1);
            Assert.Equal(i, r.Right);
            Assert.Equal(i + 1, r.Bottom);
            SKRect skrect = RectToSkRect(r);
            Assert.Equal(i, skrect.Right);
            Assert.Equal(i + 1, skrect.Bottom);
        }
    }
}
