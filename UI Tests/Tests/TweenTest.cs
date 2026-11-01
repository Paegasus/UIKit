using Xunit;

using SkiaSharp;

using UI.GFX.Animation;
using UI.GFX.Geometry;

using static UI.Tests.GeometryUtil;

namespace UI.Tests;

public static class TweenTest
{
    // Steps one unit of least precision towards positive infinity, matching the
    // non-Windows next_double() used by the upstream tests.
    private static double NextDouble(double d) => Math.BitIncrement(d);

    private static void AssertSkColorEqual(uint expected, in SKColor actual)
        => Assert.Equal(expected, (uint)actual);

    // Validates that the same interpolations are made as in Blink.
    [Fact]
    private static void TestColorValueBetween()
    {
        // From blink's AnimatableColorTest.
        AssertSkColorEqual(0xFF00FF00, Tween.ColorValueBetween(-10.0, 0xFF00FF00, 0xFF00FF00));
        AssertSkColorEqual(0xFF00FF00, Tween.ColorValueBetween(-10.0, 0xFF00FF00, 0xFFFF00FF));
        AssertSkColorEqual(0xFF00FF00, Tween.ColorValueBetween(0.0, 0xFF00FF00, 0xFFFF00FF));
        AssertSkColorEqual(0xFF01FE01, Tween.ColorValueBetween(1.0 / 255, 0xFF00FF00, 0xFFFF00FF));
        AssertSkColorEqual(0xFF808080, Tween.ColorValueBetween(0.5, 0xFF00FF00, 0xFFFF00FF));
        AssertSkColorEqual(0xFFFE01FE,
                           Tween.ColorValueBetween(254.0 / 255.0, 0xFF00FF00, 0xFFFF00FF));
        AssertSkColorEqual(0xFFFF00FF, Tween.ColorValueBetween(1.0, 0xFF00FF00, 0xFFFF00FF));
        AssertSkColorEqual(0xFFFF00FF, Tween.ColorValueBetween(10.0, 0xFF00FF00, 0xFFFF00FF));
        AssertSkColorEqual(0xFF0C253E,
                           Tween.ColorValueBetween(3.0 / 16.0, 0xFF001020, 0xFF4080C0));
        AssertSkColorEqual(0x80FF00FF, Tween.ColorValueBetween(0.5, 0x0000FF00, 0xFFFF00FF));
        AssertSkColorEqual(0x60AA55AA, Tween.ColorValueBetween(0.5, 0x4000FF00, 0x80FF00FF));
        AssertSkColorEqual(0x60FFAAFF, Tween.ColorValueBetween(0.5, 0x40FF00FF, 0x80FFFFFF));
        AssertSkColorEqual(0x103060A0, Tween.ColorValueBetween(0.5, 0x10204080, 0x104080C0));
    }

    [Fact]
    private static void TestColorValueBetweenFloat()
    {
        // Mirrors the 0x0000FF00 -> 0xFFFF00FF case above in premultiplied
        // float space: 50% of a transparent green and an opaque magenta.
        SKColorF result = Tween.ColorValueBetween(
            0.5, new SKColorF(0f, 1f, 0f, 0f), new SKColorF(1f, 0f, 1f, 1f));

        Assert.Equal(1f, result.Red, 1e-6f);
        Assert.Equal(0f, result.Green, 1e-6f);
        Assert.Equal(1f, result.Blue, 1e-6f);
        Assert.Equal(0.5f, result.Alpha, 1e-6f);

        // Fully transparent endpoints collapse to transparent.
        Assert.Equal(SKColorF.Empty,
                     Tween.ColorValueBetween(0.5, SKColorF.Empty, SKColorF.Empty));

        // Blended alpha is clamped to 1 when extrapolating.
        SKColorF extrapolated = Tween.ColorValueBetween(
            10.0, new SKColorF(0f, 0f, 0f, 1f), new SKColorF(1f, 1f, 1f, 1f));
        Assert.Equal(1f, extrapolated.Alpha, 1e-6f);
    }

    // Ensures that each of the 3 integers in [0, 1, 2] are selected with equal
    // weight.
    [Fact]
    private static void TestIntValueBetween()
    {
        Assert.Equal(0, Tween.IntValueBetween(0.0, 0, 2));
        Assert.Equal(0, Tween.IntValueBetween(0.5 / 3.0, 0, 2));
        Assert.Equal(0, Tween.IntValueBetween(1.0 / 3.0, 0, 2));

        Assert.Equal(1, Tween.IntValueBetween(NextDouble(1.0 / 3.0), 0, 2));
        Assert.Equal(1, Tween.IntValueBetween(1.5 / 3.0, 0, 2));
        Assert.Equal(1, Tween.IntValueBetween(2.0 / 3.0, 0, 2));

        Assert.Equal(2, Tween.IntValueBetween(NextDouble(2.0 / 3.0), 0, 2));
        Assert.Equal(2, Tween.IntValueBetween(2.5 / 3.0, 0, 2));
        Assert.Equal(2, Tween.IntValueBetween(3.0 / 3.0, 0, 2));
    }

    [Fact]
    private static void TestIntValueBetweenNegative()
    {
        Assert.Equal(-2, Tween.IntValueBetween(0.0, -2, 0));
        Assert.Equal(-2, Tween.IntValueBetween(0.5 / 3.0, -2, 0));
        Assert.Equal(-2, Tween.IntValueBetween(1.0 / 3.0, -2, 0));

        Assert.Equal(-1, Tween.IntValueBetween(NextDouble(1.0 / 3.0), -2, 0));
        Assert.Equal(-1, Tween.IntValueBetween(1.5 / 3.0, -2, 0));
        Assert.Equal(-1, Tween.IntValueBetween(2.0 / 3.0, -2, 0));

        Assert.Equal(0, Tween.IntValueBetween(NextDouble(2.0 / 3.0), -2, 0));
        Assert.Equal(0, Tween.IntValueBetween(2.5 / 3.0, -2, 0));
        Assert.Equal(0, Tween.IntValueBetween(3.0 / 3.0, -2, 0));
    }

    [Fact]
    private static void TestIntValueBetweenReverse()
    {
        Assert.Equal(2, Tween.IntValueBetween(0.0, 2, 0));
        Assert.Equal(2, Tween.IntValueBetween(0.5 / 3.0, 2, 0));
        Assert.Equal(2, Tween.IntValueBetween(1.0 / 3.0, 2, 0));

        Assert.Equal(1, Tween.IntValueBetween(NextDouble(1.0 / 3.0), 2, 0));
        Assert.Equal(1, Tween.IntValueBetween(1.5 / 3.0, 2, 0));
        Assert.Equal(1, Tween.IntValueBetween(2.0 / 3.0, 2, 0));

        Assert.Equal(0, Tween.IntValueBetween(NextDouble(2.0 / 3.0), 2, 0));
        Assert.Equal(0, Tween.IntValueBetween(2.5 / 3.0, 2, 0));
        Assert.Equal(0, Tween.IntValueBetween(3.0 / 3.0, 2, 0));
    }

    [Fact]
    private static void TestLinearIntValueBetween()
    {
        Assert.Equal(0, Tween.LinearIntValueBetween(0.0, 0, 2));
        Assert.Equal(0, Tween.LinearIntValueBetween(0.5 / 4.0, 0, 2));
        Assert.Equal(0, Tween.LinearIntValueBetween(0.99 / 4.0, 0, 2));

        Assert.Equal(1, Tween.LinearIntValueBetween(1.0 / 4.0, 0, 2));
        Assert.Equal(1, Tween.LinearIntValueBetween(1.5 / 4.0, 0, 2));
        Assert.Equal(1, Tween.LinearIntValueBetween(2.0 / 4.0, 0, 2));
        Assert.Equal(1, Tween.LinearIntValueBetween(2.5 / 4.0, 0, 2));
        Assert.Equal(1, Tween.LinearIntValueBetween(2.99 / 4.0, 0, 2));

        Assert.Equal(2, Tween.LinearIntValueBetween(3.0 / 4.0, 0, 2));
        Assert.Equal(2, Tween.LinearIntValueBetween(3.5 / 4.0, 0, 2));
        Assert.Equal(2, Tween.LinearIntValueBetween(4.0 / 4.0, 0, 2));
    }

    [Fact]
    private static void TestLinearIntValueBetweenNegative()
    {
        Assert.Equal(-2, Tween.LinearIntValueBetween(0.0, -2, 0));
        Assert.Equal(-2, Tween.LinearIntValueBetween(0.5 / 4.0, -2, 0));
        Assert.Equal(-2, Tween.LinearIntValueBetween(0.99 / 4.0, -2, 0));

        Assert.Equal(-1, Tween.LinearIntValueBetween(1.0 / 4.0, -2, 0));
        Assert.Equal(-1, Tween.LinearIntValueBetween(1.5 / 4.0, -2, 0));
        Assert.Equal(-1, Tween.LinearIntValueBetween(2.0 / 4.0, -2, 0));
        Assert.Equal(-1, Tween.LinearIntValueBetween(2.5 / 4.0, -2, 0));
        Assert.Equal(-1, Tween.LinearIntValueBetween(2.99 / 4.0, -2, 0));

        Assert.Equal(0, Tween.LinearIntValueBetween(3.0 / 4.0, -2, 0));
        Assert.Equal(0, Tween.LinearIntValueBetween(3.5 / 4.0, -2, 0));
        Assert.Equal(0, Tween.LinearIntValueBetween(4.0 / 4.0, -2, 0));
    }

    [Fact]
    private static void TestClampedFloatValueBetween()
    {
        const float v1 = 10.0f;
        const float v2 = 20.0f;

        TimeSpan from = TimeSpan.FromSeconds(1);
        TimeSpan to = TimeSpan.FromSeconds(2);

        TimeSpan tBefore = TimeSpan.FromSeconds(0.9);
        TimeSpan tBetween = TimeSpan.FromSeconds(1.6);
        TimeSpan tAfter = TimeSpan.FromSeconds(2.2);

        Assert.Equal(v1, Tween.ClampedFloatValueBetween(tBefore, from, v1, to, v2));
        Assert.Equal(16.0f, Tween.ClampedFloatValueBetween(tBetween, from, v1, to, v2), 3);
        Assert.Equal(v2, Tween.ClampedFloatValueBetween(tAfter, from, v1, to, v2));
    }

    [Fact]
    private static void TestClampedFloatValueBetweenUsesTweenType()
    {
        TimeSpan from = TimeSpan.Zero;
        TimeSpan to = TimeSpan.FromSeconds(1);

        // EaseInOut2 is symmetric, so the midpoint is exactly 0.5.
        Assert.Equal(15.0f,
                     Tween.ClampedFloatValueBetween(
                         TimeSpan.FromSeconds(0.5), from, 10.0f, to, 20.0f,
                         Tween.TweenType.EaseInOut2),
                     3);
    }

    // Verifies the corners of the rect are animated, rather than the
    // origin/size (which would result in different rounding).
    [Fact]
    private static void TestRectValueBetween()
    {
        Rect r1 = new(0, 0, 10, 10);
        Rect r2 = new(10, 10, 30, 30);

        Rect tweened = Tween.RectValueBetween(0.08, r1, r2);
        Assert.True(new Rect(1, 1, 11, 11) == tweened,
                    $"Expected (1, 1, 11, 11), got ({tweened.X}, {tweened.Y}, {tweened.Width}, {tweened.Height})");
    }

    [Fact]
    private static void TestRectFValueBetween()
    {
        RectF r1 = new(0, 0, 10, 10);
        RectF r2 = new(10, 10, 30, 30);

        // Unlike the integer version, float rects are not corner-rounded.
        AssertRectFEqual(new RectF(0.8f, 0.8f, 11.6f, 11.6f),
                         Tween.RectFValueBetween(0.08, r1, r2));
    }

    [Fact]
    private static void TestSizeValueBetween()
    {
        Size size1 = new(12, 24);
        Size size2 = new(36, 48);

        const double kBefore = -0.125;
        const double kFrom = 0.0;
        const double kBetween = 0.5;
        const double kTo = 1.0;
        const double kAfter = 1.125;

        Assert.True(new Size(9, 21) == Tween.SizeValueBetween(kBefore, size1, size2));
        Assert.True(size1 == Tween.SizeValueBetween(kFrom, size1, size2));
        Assert.True(new Size(24, 36) == Tween.SizeValueBetween(kBetween, size1, size2));
        Assert.True(size2 == Tween.SizeValueBetween(kTo, size1, size2));
        Assert.True(new Size(39, 51) == Tween.SizeValueBetween(kAfter, size1, size2));
    }

    [Fact]
    private static void TestSizeValueBetweenClampedExtrapolation()
    {
        Size size1 = new(0, 0);
        Size size2 = new(36, 48);

        const double kBefore = -1.0;

        // We should not extrapolate in this case as it would result in a
        // negative and invalid size.
        Assert.True(size1 == Tween.SizeValueBetween(kBefore, size1, size2));
    }

    [Fact]
    private static void TestSizeFValueBetween()
    {
        SizeF size1 = new(12.0f, 24.0f);
        SizeF size2 = new(36.0f, 48.0f);

        const double kBefore = -0.125;
        const double kFrom = 0.0;
        const double kBetween = 0.5;
        const double kTo = 1.0;
        const double kAfter = 1.125;

        Assert.Equal(new SizeF(9.0f, 21.0f), Tween.SizeFValueBetween(kBefore, size1, size2));
        Assert.Equal(size1, Tween.SizeFValueBetween(kFrom, size1, size2));
        Assert.Equal(new SizeF(24.0f, 36.0f), Tween.SizeFValueBetween(kBetween, size1, size2));
        Assert.Equal(size2, Tween.SizeFValueBetween(kTo, size1, size2));
        Assert.Equal(new SizeF(39.0f, 51.0f), Tween.SizeFValueBetween(kAfter, size1, size2));
    }

    [Fact]
    private static void TestSizeFValueBetweenClampedExtrapolation()
    {
        SizeF size1 = new(0.0f, 0.0f);
        SizeF size2 = new(36.0f, 48.0f);

        const double kBefore = -1.0;

        // We should not extrapolate in this case as it would result in a
        // negative and invalid size.
        Assert.Equal(size1, Tween.SizeFValueBetween(kBefore, size1, size2));
    }

    [Fact]
    private static void TestTransformValueBetween()
    {
        Transform start = Transform.MakeTranslation(10, 20);
        Transform target = Transform.MakeTranslation(30, 60);

        AssertTransformNear(start, Tween.TransformValueBetween(-1.0, start, target), 1e-5f);
        AssertTransformNear(target, Tween.TransformValueBetween(2.0, start, target), 1e-5f);
        AssertTransformNear(Transform.MakeTranslation(20, 40),
                            Tween.TransformValueBetween(0.5, start, target), 1e-5f);
    }

    [Fact]
    private static void TestTransformOperationsValueBetween()
    {
        TransformOperations start = new();
        start.AppendTranslate(10, 20, 0);

        TransformOperations target = new();
        target.AppendTranslate(30, 60, 0);

        TransformOperations blended = Tween.TransformOperationsValueBetween(0.5, start, target);
        AssertTransformNear(Transform.MakeTranslation(20, 40), blended.Apply(), 1e-5f);
    }

    [Fact]
    private static void TestCalculateValueEndpoints()
    {
        foreach (Tween.TweenType type in Enum.GetValues<Tween.TweenType>())
        {
            double atZero = Tween.CalculateValue(type, 0.0);
            double atOne = Tween.CalculateValue(type, 1.0);

            if (type == Tween.TweenType.Zero)
            {
                Assert.Equal(0.0, atZero);
                Assert.Equal(0.0, atOne);
                continue;
            }

            Assert.Equal(0.0, atZero, 1e-7);

            if (type == Tween.TweenType.SmoothInOut)
            {
                // Upstream maps state through sin() directly rather than
                // mapping the [0, 1] range onto a sine wave, so it does not
                // reach 1.
                Assert.Equal(Math.Sin(1.0), atOne, 1e-12);
                continue;
            }

            Assert.Equal(1.0, atOne, 1e-7);
        }
    }

    [Fact]
    private static void TestCalculateValueIsMonotonic()
    {
        const int kSteps = 100;

        foreach (Tween.TweenType type in Enum.GetValues<Tween.TweenType>())
        {
            double previous = Tween.CalculateValue(type, 0.0);
            for (int i = 1; i <= kSteps; i++)
            {
                double current = Tween.CalculateValue(type, (double)i / kSteps);
                Assert.True(current >= previous - 1e-9,
                            $"{type} regressed at {i}/{kSteps}: {current} < {previous}");
                previous = current;
            }
        }
    }

    [Fact]
    private static void TestCalculateValueLinear()
    {
        Assert.Equal(0.0, Tween.CalculateValue(Tween.TweenType.Linear, 0.0));
        Assert.Equal(0.25, Tween.CalculateValue(Tween.TweenType.Linear, 0.25));
        Assert.Equal(0.5, Tween.CalculateValue(Tween.TweenType.Linear, 0.5));
        Assert.Equal(1.0, Tween.CalculateValue(Tween.TweenType.Linear, 1.0));
    }

    [Fact]
    private static void TestValueBetween()
    {
        Assert.Equal(5.0, Tween.DoubleValueBetween(0.5, 0.0, 10.0));
        Assert.Equal(5.0f, Tween.FloatValueBetween(0.5, 0.0f, 10.0f));
        Assert.Equal(15.0, Tween.DoubleValueBetween(1.5, 0.0, 10.0));
    }
}
