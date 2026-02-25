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
}
