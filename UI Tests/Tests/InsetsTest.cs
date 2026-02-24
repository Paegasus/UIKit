using Xunit;

using UI.GFX.Geometry;

namespace UI.Tests;

public static class InsetsTest
{
    [Fact]
    private static void TestDefault()
    {
        Insets insets = new();
        Assert.Equal(0, insets.Top);
        Assert.Equal(0, insets.Left);
        Assert.Equal(0, insets.Bottom);
        Assert.Equal(0, insets.Right);
    }

    [Fact]
    private static void TestTLBR()
    {
        Insets insets = Insets.TLBR(1, 2, 3, 4);
        Assert.Equal(1, insets.Top);
        Assert.Equal(2, insets.Left);
        Assert.Equal(3, insets.Bottom);
        Assert.Equal(4, insets.Right);
    }

    [Fact]
    private static void TestVH()
    {
        Insets insets = Insets.VH(1, 2);
        Assert.Equal(1, insets.Top);
        Assert.Equal(2, insets.Left);
        Assert.Equal(1, insets.Bottom);
        Assert.Equal(2, insets.Right);
    }

    [Fact]
    private static void TestSetLeftRight()
    {
        Insets insets = new(1);
        insets.SetLeftRight(3, 4);

        Assert.Equal(1, insets.Top);
        Assert.Equal(3, insets.Left);
        Assert.Equal(1, insets.Bottom);
        Assert.Equal(4, insets.Right);

        Insets insets2 = new Insets(1);
        insets2.SetLeftRight(3, 4);

        Assert.Equal(insets, insets2);
    }

    [Fact]
    private static void TestSetTopBottom()
    {
        Insets insets = new(1);
        insets.SetTopBottom(3, 4);
        Assert.Equal(3, insets.Top);
        Assert.Equal(1, insets.Left);
        Assert.Equal(4, insets.Bottom);
        Assert.Equal(1, insets.Right);

        Insets insets2 = new Insets(1);
        insets2.SetTopBottom(3, 4);

        Assert.Equal(insets, insets2);
    }

    [Fact]
    private static void TestSetTop()
    {
        Insets insets = new(1);
        insets.SetTop(2);

        Assert.Equal(2, insets.Top);
        Assert.Equal(1, insets.Left);
        Assert.Equal(1, insets.Bottom);
        Assert.Equal(1, insets.Right);

        Insets insets2 = new Insets(1);
        insets2.SetTop(2);

        Assert.Equal(insets, insets2);
    }

    [Fact]
    private static void TestSetBottom()
    {
        Insets insets = new(1);
        insets.SetBottom(2);

        Assert.Equal(1, insets.Top);
        Assert.Equal(1, insets.Left);
        Assert.Equal(2, insets.Bottom);
        Assert.Equal(1, insets.Right);

        Insets insets2 = new Insets(1);
        insets2.SetBottom(2);

        Assert.Equal(insets, insets2);
    }

    [Fact]
    private static void TestSetLeft()
    {
        Insets insets = new(1);
        insets.SetLeft(2);

        Assert.Equal(1, insets.Top);
        Assert.Equal(2, insets.Left);
        Assert.Equal(1, insets.Bottom);
        Assert.Equal(1, insets.Right);

        Insets insets2 = new Insets(1);
        insets2.SetLeft(2);

        Assert.Equal(insets, insets2);
    }

    [Fact]
    private static void TestSetRight()
    {
        Insets insets = new(1);
        insets.SetRight(2);

        Assert.Equal(1, insets.Top);
        Assert.Equal(1, insets.Left);
        Assert.Equal(1, insets.Bottom);
        Assert.Equal(2, insets.Right);

        Insets insets2 = new Insets(1);
        insets2.SetRight(2);

        Assert.Equal(insets, insets2);
    }

    [Fact]
    private static void TestWidthHeightAndIsEmpty()
    {
        Insets insets = new();
        Assert.Equal(0, insets.Width);
        Assert.Equal(0, insets.Height);
        Assert.True(insets.IsEmpty);

        insets.SetLeftRight(3, 4);
        Assert.Equal(7, insets.Width);
        Assert.Equal(0, insets.Height);
        Assert.False(insets.IsEmpty);

        insets.SetLeftRight(0, 0);
        insets.SetTopBottom(1, 2);
        Assert.Equal(0, insets.Width);
        Assert.Equal(3, insets.Height);
        Assert.False(insets.IsEmpty);

        insets.SetLeftRight(4, 5);
        Assert.Equal(9, insets.Width);
        Assert.Equal(3, insets.Height);
        Assert.False(insets.IsEmpty);
    }

    [Fact]
    private static void TestTranspose()
    {
        Insets insets = Insets.TLBR(1, 2, 3, 4);
        insets.Transpose();
        Assert.Equal(insets, Insets.TLBR(2, 1, 4, 3));

        insets = new Insets();
        insets.Transpose();
        Assert.Equal(insets, new Insets());
    }

    [Fact]
    private static void TestOperators()
    {
        Insets insets = new Insets();
        insets.SetLeftRight(2, 4);
        insets.SetTopBottom(1, 3);

        Insets insets2 = new Insets();
        insets2.SetLeftRight(6, 8);
        insets2.SetTopBottom(5, 7);

        insets += insets2;

        Assert.Equal(6, insets.Top);
        Assert.Equal(8, insets.Left);
        Assert.Equal(10, insets.Bottom);
        Assert.Equal(12, insets.Right);

        insets2 = new Insets();
        insets2.SetLeftRight(0, 2);
        insets2.SetTopBottom(-1, 1);

        insets -= insets2;
        
        Assert.Equal(7, insets.Top);
        Assert.Equal(8, insets.Left);
        Assert.Equal(9, insets.Bottom);
        Assert.Equal(10, insets.Right);

        insets2 = new Insets();
        insets2.SetLeftRight(5, -20);
        insets2.SetTopBottom(10, 0);

        insets = new Insets(10) + insets2;

        Assert.Equal(20, insets.Top);
        Assert.Equal(15, insets.Left);
        Assert.Equal(10, insets.Bottom);
        Assert.Equal(-10, insets.Right);

        insets2 = new Insets();
        insets2.SetLeftRight(5, -20);
        insets2.SetTopBottom(10, 0);

        insets = new Insets(10) - insets2;
        Assert.Equal(0, insets.Top);
        Assert.Equal(5, insets.Left);
        Assert.Equal(10, insets.Bottom);
        Assert.Equal(30, insets.Right);
    }

    [Fact]
    private static void TestEquality()
    {
        Insets insets1 = new Insets();
        insets1.SetLeftRight(2, 4);
        insets1.SetTopBottom(1, 3);

        Insets insets2 = new();

        // Test operator== and operator!=.
        Assert.False(insets1 == insets2);
        Assert.True(insets1 != insets2);

        insets2.SetLeftRight(2, 4);
        insets2.SetTopBottom(1, 3);

        Assert.True(insets1 == insets2);
        Assert.False(insets1 != insets2);
    }

    [Fact]
    private static void TestToString()
    {
        Insets insets = new Insets();
        insets.SetLeftRight(2, 4);
        insets.SetTopBottom(1, 3);
        Assert.Equal("x:2,4 y:1,3", insets.ToString());
    }

    [Fact]
    private static void TestOffset()
    {
        Insets insets = new Insets();
        insets.SetLeftRight(2, 4);
        insets.SetTopBottom(1, 3);

        Rect rect = new(5, 6, 7, 8);
        Vector2D vector = new(9, 10);

        // Whether you inset then offset the rect, offset then inset the rect, or
        // offset the insets then apply to the rect, the outcome should be the same.
        Rect inset_first = rect;
        inset_first.Inset(insets);
        inset_first.Offset(vector);

        Rect offset_first = rect;
        offset_first.Offset(vector);
        offset_first.Inset(insets);

        Insets insets_with_offset = insets;
        insets_with_offset.Offset(vector);

        Insets expected = new Insets();
        expected.SetLeftRight(11, -5);
        expected.SetTopBottom(11, -7);

        Assert.Equal(expected, insets_with_offset);
        Assert.Equal(insets_with_offset, insets + vector);

        Rect inset_by_offset = rect;
        inset_by_offset.Inset(insets_with_offset);

        Assert.Equal(inset_first, offset_first);
        Assert.Equal(inset_by_offset, inset_first);
    }

    [Fact]
    private static void TestScale()
    {
        Insets insets = new();
        insets.SetLeftRight(5, 1);
        insets.SetTopBottom(7, 3);

        Insets test = Insets.ScaleToFlooredInsets(insets, 2.5f, 3.5f);
        Insets expected = new();
        expected.SetLeftRight(12, 2);
        expected.SetTopBottom(24, 10);
        Assert.Equal(expected, test);

        test = Insets.ScaleToFlooredInsets(insets, 2.5f);
        expected = new Insets();
        expected.SetLeftRight(12, 2);
        expected.SetTopBottom(17, 7);
        Assert.Equal(expected, test);

        test = Insets.ScaleToCeiledInsets(insets, 2.5f, 3.5f);
        expected = new Insets();
        expected.SetLeftRight(13, 3);
        expected.SetTopBottom(25, 11);
        Assert.Equal(expected, test);

        test = Insets.ScaleToCeiledInsets(insets, 2.5f);
        expected = new Insets();
        expected.SetLeftRight(13, 3);
        expected.SetTopBottom(18, 8);
        Assert.Equal(expected, test);

        test = Insets.ScaleToRoundedInsets(insets, 2.49f, 3.49f);
        expected = new Insets();
        expected.SetLeftRight(12, 2);
        expected.SetTopBottom(24, 10);
        Assert.Equal(expected, test);

        test = Insets.ScaleToRoundedInsets(insets, 2.49f);
        expected = new Insets();
        expected.SetLeftRight(12, 2);
        expected.SetTopBottom(17, 7);
        Assert.Equal(expected, test);

        test = Insets.ScaleToRoundedInsets(insets, 2.5f, 3.5f);
        expected = new Insets();
        expected.SetLeftRight(13, 3);
        expected.SetTopBottom(25, 11);
        Assert.Equal(expected, test);

        test = Insets.ScaleToRoundedInsets(insets, 2.5f);
        expected = new Insets();
        expected.SetLeftRight(13, 3);
        expected.SetTopBottom(18, 8);
        Assert.Equal(expected, test);
    }

    [Fact]
    private static void TestScaleNegative()
    {
        Insets insets = new();
        insets.SetLeftRight(-5, -1);
        insets.SetTopBottom(-7, -3);

        Insets test = Insets.ScaleToFlooredInsets(insets, 2.5f, 3.5f);
        Insets expected = new();
        expected.SetLeftRight(-13, -3);
        expected.SetTopBottom(-25, -11);
        Assert.Equal(expected, test);

        test = Insets.ScaleToFlooredInsets(insets, 2.5f);
        expected = new Insets();
        expected.SetLeftRight(-13, -3);
        expected.SetTopBottom(-18, -8);
        Assert.Equal(expected, test);

        test = Insets.ScaleToCeiledInsets(insets, 2.5f, 3.5f);
        expected = new Insets();
        expected.SetLeftRight(-12, -2);
        expected.SetTopBottom(-24, -10);
        Assert.Equal(expected, test);

        test = Insets.ScaleToCeiledInsets(insets, 2.5f);
        expected = new Insets();
        expected.SetLeftRight(-12, -2);
        expected.SetTopBottom(-17, -7);
        Assert.Equal(expected, test);

        test = Insets.ScaleToRoundedInsets(insets, 2.49f, 3.49f);
        expected = new Insets();
        expected.SetLeftRight(-12, -2);
        expected.SetTopBottom(-24, -10);
        Assert.Equal(expected, test);

        test = Insets.ScaleToRoundedInsets(insets, 2.49f);
        expected = new Insets();
        expected.SetLeftRight(-12, -2);
        expected.SetTopBottom(-17, -7);
        Assert.Equal(expected, test);

        test = Insets.ScaleToRoundedInsets(insets, 2.5f, 3.5f);
        expected = new Insets();
        expected.SetLeftRight(-13, -3);
        expected.SetTopBottom(-25, -11);
        Assert.Equal(expected, test);

        test = Insets.ScaleToRoundedInsets(insets, 2.5f);
        expected = new Insets();
        expected.SetLeftRight(-13, -3);
        expected.SetTopBottom(-18, -8);
        Assert.Equal(expected, test);
    }

    [Fact]
    private static void TestIntegerOverflow()
    {
        int int_min = int.MinValue;
        int int_max = int.MaxValue;

        Insets width_height_test = new(int_max);
        Assert.Equal(int_max, width_height_test.Width);
        Assert.Equal(int_max, width_height_test.Height);

        Insets plus_test = new(int_max);
        plus_test += new Insets(int_max);
        Assert.Equal(new Insets(int_max), plus_test);

        Insets negation_test = -new Insets(int_min);
        Assert.Equal(new Insets(int_max), negation_test);

        Insets scale_test = new(int_max);
        scale_test = Insets.ScaleToRoundedInsets(scale_test, 2.0f);
        Assert.Equal(new Insets(int_max), scale_test);
    }

    [Fact]
    private static void TestIntegerUnderflow()
    {
        int int_min = int.MinValue;
        int int_max = int.MaxValue;

        Insets width_height_test = new(int_min);
        Assert.Equal(int_min, width_height_test.Width);
        Assert.Equal(int_min, width_height_test.Height);

        Insets minus_test = new(int_min);
        minus_test -= new Insets(int_max);
        Assert.Equal(new Insets(int_min), minus_test);

        Insets scale_test = new(int_min);
        scale_test = Insets.ScaleToRoundedInsets(scale_test, 2.0f);
        Assert.Equal(new Insets(int_min), scale_test);
    }
}
