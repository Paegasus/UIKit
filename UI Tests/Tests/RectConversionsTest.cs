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
        Assert.Equal(new Rect(-1, -1, 2, 2), ToEnclosedRect(new RectF(-1.5f, -1.5f, 3.0f, 3.0f)));
        Assert.Equal(new Rect(-1, -1, 3, 3), ToEnclosedRect(new RectF(-1.5f, -1.5f, 3.5f, 3.5f)));
        Assert.Equal(new Rect(kMaxInt, kMaxInt, 0, 0), ToEnclosedRect(new RectF(kMaxFloat, kMaxFloat, 2.0f, 2.0f)));
        Assert.Equal(new Rect(0, 0, kMaxInt, kMaxInt), ToEnclosedRect(new RectF(0.0f, 0.0f, kMaxFloat, kMaxFloat)));
        Assert.Equal(new Rect(20001, 20001, 0, 0), ToEnclosedRect(new RectF(20000.5f, 20000.5f, 0.5f, 0.5f)));
        Assert.Equal(new Rect(kMaxInt, kMaxInt, 0, 0), ToEnclosedRect(new RectF(kMaxIntF, kMaxIntF, kMaxIntF, kMaxIntF)));
        Assert.Equal(new Rect(2, 3, 5, 5), ToEnclosedRect(new RectF(1.9999f, 2.0002f, 5.9998f, 6.0001f)));
        Assert.Equal(new Rect(2, 3, 6, 4), ToEnclosedRect(new RectF(1.9999f, 2.0001f, 6.0002f, 5.9998f)));
        Assert.Equal(new Rect(2, 3, 5, 5), ToEnclosedRect(new RectF(1.9998f, 2.0002f, 6.0001f, 5.9999f)));
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

    [Fact]
    private static void TestToEnclosingRect()
    {
        Assert.Equal(new Rect(), ToEnclosingRect(new RectF()));
        Assert.Equal(new Rect(5, 5, 0, 0), ToEnclosingRect(new RectF(5.5f, 5.5f, 0.0f, 0.0f)));
        Assert.Equal(new Rect(3, 2, 0, 0), ToEnclosingRect(new RectF(3.5f, 2.5f, kEpsilonFloat, -0.0f)));
        Assert.Equal(new Rect(3, 2, 0, 1), ToEnclosingRect(new RectF(3.5f, 2.5f, 0.0f, 0.001f)));
        Assert.Equal(new Rect(-2, -2, 4, 4), ToEnclosingRect(new RectF(-1.5f, -1.5f, 3.0f, 3.0f)));
        Assert.Equal(new Rect(-2, -2, 4, 4), ToEnclosingRect(new RectF(-1.5f, -1.5f, 3.5f, 3.5f)));
        Assert.Equal(new Rect(kMaxInt, kMaxInt, 0, 0), ToEnclosingRect(new RectF(kMaxFloat, kMaxFloat, 2.0f, 2.0f)));
        Assert.Equal(new Rect(0, 0, kMaxInt, kMaxInt), ToEnclosingRect(new RectF(0.0f, 0.0f, kMaxFloat, kMaxFloat)));
        Assert.Equal(new Rect(20000, 20000, 1, 1), ToEnclosingRect(new RectF(20000.5f, 20000.5f, 0.5f, 0.5f)));
        Assert.Equal(new Rect(kMaxInt, kMaxInt, 0, 0), ToEnclosingRect(new RectF(kMaxIntF, kMaxIntF, kMaxIntF, kMaxIntF)));
        Assert.Equal(new Rect(-1, -1, 22777713, 2), ToEnclosingRect(new RectF(-0.5f, -0.5f, 22777712.0f, 1.0f)));
        Assert.Equal(new Rect(1, 2, 7, 7), ToEnclosingRect(new RectF(1.9999f, 2.0002f, 5.9998f, 6.0001f)));
        Assert.Equal(new Rect(1, 2, 8, 6), ToEnclosingRect(new RectF(1.9999f, 2.0001f, 6.0002f, 5.9998f)));
        Assert.Equal(new Rect(1, 2, 7, 7), ToEnclosingRect(new RectF(1.9998f, 2.0002f, 6.0001f, 5.9999f)));
    }

    [Fact]
    private static void TestToEnclosingRectHugeRectF()
    {
        RectF source = new(kMinIntF, kMinIntF, kMaxIntF* 3.0f, kMaxIntF * 3.0f);
        Rect enclosing = ToEnclosingRect(source);

        // That rect can't be represented, but it should be big.
        Assert.Equal(kMaxInt, enclosing.Width);
        Assert.Equal(kMaxInt, enclosing.Height);
        // It should include some axis near the global origin.
        Assert.True(1 > enclosing.X);
        Assert.True(1 > enclosing.Y);
        // And it should cause computation issues for itself.
        Assert.True(0 < enclosing.Right);
        Assert.True(0 < enclosing.Bottom);
    }

    [Fact]
    private static void TestToEnclosingRectIgnoringError()
    {
        float kError = 0.001f;
        Assert.Equal(new Rect(), ToEnclosingRectIgnoringError(new RectF(), kError));
        Assert.Equal(new Rect(5, 5, 0, 0), ToEnclosingRectIgnoringError(new RectF(5.5f, 5.5f, 0.0f, 0.0f), kError));
        Assert.Equal(new Rect(3, 2, 0, 0), ToEnclosingRectIgnoringError(new RectF(3.5f, 2.5f, kEpsilonFloat, -0.0f), kError));
        Assert.Equal(new Rect(3, 2, 0, 1), ToEnclosingRectIgnoringError(new RectF(3.5f, 2.5f, 0.0f, 0.001f), kError));
        Assert.Equal(new Rect(-2, -2, 4, 4), ToEnclosingRectIgnoringError(new RectF(-1.5f, -1.5f, 3.0f, 3.0f), kError));
        Assert.Equal(new Rect(-2, -2, 4, 4), ToEnclosingRectIgnoringError(new RectF(-1.5f, -1.5f, 3.5f, 3.5f), kError));
        Assert.Equal(new Rect(kMaxInt, kMaxInt, 0, 0), ToEnclosingRectIgnoringError(new RectF(kMaxFloat, kMaxFloat, 2.0f, 2.0f), kError));
        Assert.Equal(new Rect(0, 0, kMaxInt, kMaxInt), ToEnclosingRectIgnoringError(new RectF(0.0f, 0.0f, kMaxFloat, kMaxFloat), kError));
        Assert.Equal(new Rect(20000, 20000, 1, 1), ToEnclosingRectIgnoringError(new RectF(20000.5f, 20000.5f, 0.5f, 0.5f),kError));
        Assert.Equal(new Rect(kMaxInt, kMaxInt, 0, 0), ToEnclosingRectIgnoringError(new RectF(kMaxIntF, kMaxIntF, kMaxIntF, kMaxIntF), kError));
        Assert.Equal(new Rect(-1, -1, 22777713, 2), ToEnclosingRectIgnoringError(new RectF(-0.5f, -0.5f, 22777712.0f, 1.0f),kError));
        Assert.Equal(new Rect(2, 2, 6, 6), ToEnclosingRectIgnoringError(new RectF(1.9999f, 2.0002f, 5.9998f, 6.0001f), kError));
        Assert.Equal(new Rect(2, 2, 6, 6), ToEnclosingRectIgnoringError(new RectF(1.9999f, 2.0001f, 6.0002f, 5.9998f), kError));
        Assert.Equal(new Rect(2, 2, 6, 6), ToEnclosingRectIgnoringError(new RectF(1.9998f, 2.0002f, 6.0001f, 5.9999f), kError));
    }

    [Fact]
    private static void TestToNearestRect()
    {
        Rect rect = new();
        Assert.Equal(rect, ToNearestRect(new RectF(rect)));

        rect = new Rect(-1, -1, 3, 3);
        Assert.Equal(rect, ToNearestRect(new RectF(rect)));

        RectF rectf = new(-1.00001f, -0.999999f, 3.0000001f, 2.999999f);
        Assert.Equal(rect, ToNearestRect(rectf));
    }

    [Fact]
    private static void TestToFlooredRect()
    {
        Assert.Equal(new Rect(), ToFlooredRectDeprecated(new RectF()));
        Assert.Equal(new Rect(-2, -2, 3, 3), ToFlooredRectDeprecated(new RectF(-1.5f, -1.5f, 3.0f, 3.0f)));
        Assert.Equal(new Rect(-2, -2, 3, 3), ToFlooredRectDeprecated(new RectF(-1.5f, -1.5f, 3.5f, 3.5f)));
        Assert.Equal(new Rect(20000, 20000, 0, 0), ToFlooredRectDeprecated(new RectF(20000.5f, 20000.5f, 0.5f, 0.5f)));
    }
}
