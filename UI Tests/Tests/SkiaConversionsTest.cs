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

    [Fact]
    private static void TestSkIRectToRectClamping()
    {
        // This clamping only makes sense if SkIRect and Rect have the same size.
        // Otherwise, either other overflows can occur that we don't handle,
        // or no overflows can occur.
        //
        // No need to do the check, int is always System.Int32 in C#
        //if (sizeof(int) != sizeof(System.Int32))
        //    return;

        var int_max = int.MaxValue;
        var int_min = int.MinValue;

        // right-left and bottom-top would overflow.
        // These should be mapped to max width/height, which is as close as Rect can represent.
        Rect result = SkIRectToRect(new SKRectI(int_min, int_min, int_max, int_max));
        Assert.Equal(new Size(int_max, int_max), result.Size);

        // right-left and bottom-top would underflow.
        // These should be mapped to zero, like all negative values.
        result = SkIRectToRect(new SKRectI(int_max, int_max, int_min, int_min));
        Assert.Equal(new Rect(int_max, int_max, 0, 0), result);
    }

    [Fact]
    private static void TestTransformSkM44Conversions()
    {
        ReadOnlySpan<float> v =
        [
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        ];

        Transform t = Transform.ColMajorF(v);

        SKMatrix44 m = TransformToSkM44(t);

        Span<float> v1 = stackalloc float[16]; 
        m.ToRowMajor(v1);

        //Assert.True(v.SequenceEqual(v1));
        Assert.Equal(v, v1);
        Assert.Equal(t, SkM44ToTransform(m));
    }
}
