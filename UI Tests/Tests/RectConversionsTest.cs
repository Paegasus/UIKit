using Xunit;

using UI.GFX.Geometry;
using UI.Extensions;

using static UI.GFX.Geometry.RectConversions;

namespace UI.Tests;

public static class RectConversionsTest
{
    private static int kMaxInt = int.MaxValue;
    private static int kMinInt = int.MinValue;
    private static float kMaxFloat = float.MaxValue;
    private static float kEpsilonFloat = float.MachineEpsilon;
    private static float kMaxIntF = (float)kMaxInt;
    private static float kMinIntF = (float)kMinInt;

    [Fact]
    private static void TestToEnclosedRect()
    {
        Assert.Equal(new Rect(), ToEnclosedRect(new RectF()));
        Assert.Equal(new Rect(-1, -1, 2, 2),
                  ToEnclosedRect(new RectF(-1.5f, -1.5f, 3.0f, 3.0f)));
        Assert.Equal(new Rect(-1, -1, 3, 3),
                  ToEnclosedRect(new RectF(-1.5f, -1.5f, 3.5f, 3.5f)));
        Assert.Equal(new Rect(kMaxInt, kMaxInt, 0, 0),
                  ToEnclosedRect(new RectF(kMaxFloat, kMaxFloat, 2.0f, 2.0f)));
        Assert.Equal(new Rect(0, 0, kMaxInt, kMaxInt),
                  ToEnclosedRect(new RectF(0.0f, 0.0f, kMaxFloat, kMaxFloat)));
        Assert.Equal(new Rect(20001, 20001, 0, 0),
                  ToEnclosedRect(new RectF(20000.5f, 20000.5f, 0.5f, 0.5f)));
        Assert.Equal(new Rect(kMaxInt, kMaxInt, 0, 0),
                  ToEnclosedRect(new RectF(kMaxIntF, kMaxIntF, kMaxIntF, kMaxIntF)));
        Assert.Equal(new Rect(2, 3, 5, 5),
                  ToEnclosedRect(new RectF(1.9999f, 2.0002f, 5.9998f, 6.0001f)));
        Assert.Equal(new Rect(2, 3, 6, 4),
                  ToEnclosedRect(new RectF(1.9999f, 2.0001f, 6.0002f, 5.9998f)));
        Assert.Equal(new Rect(2, 3, 5, 5),
                  ToEnclosedRect(new RectF(1.9998f, 2.0002f, 6.0001f, 5.9999f)));
    }

    [Fact]
    private static void TestToEnclosedRectHugeRectF()
    {
        RectF source = new(kMinIntF, kMinIntF, kMaxIntF* 3.0f, kMaxIntF * 3.0f);
        Rect enclosed = ToEnclosedRect(source);

        // That rect can't be represented, but it should be big.
        Assert.Equal(kMaxInt, enclosed.Width);
        Assert.Equal(kMaxInt, enclosed.Height);
        // It should include some axis near the global origin.
        Assert.True(1 > enclosed.X);
        Assert.True(1 > enclosed.Y);
        // And it should not cause computation issues for itself.
        Assert.True(0 < enclosed.Right);
        Assert.True(0 < enclosed.Bottom);
    }
}
