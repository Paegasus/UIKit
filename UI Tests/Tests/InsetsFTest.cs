using Xunit;

using UI.GFX.Geometry;
using UI.Extensions;

namespace UI.Tests;

public static class InsetsFTest
{
    [Fact]
    private static void TestDefault()
    {
        InsetsF insets = new();
        Assert.Equal(0, insets.Top);
        Assert.Equal(0, insets.Left);
        Assert.Equal(0, insets.Bottom);
        Assert.Equal(0, insets.Right);
    }

    [Fact]
    private static void TestTLBR()
    {
        InsetsF insets = InsetsF.TLBR(1.25f, 2.5f, 3.75f, 4.875f);
        Assert.Equal(1.25f, insets.Top);
        Assert.Equal(2.5f, insets.Left);
        Assert.Equal(3.75f, insets.Bottom);
        Assert.Equal(4.875f, insets.Right);
    }

    [Fact]
    private static void TestVH()
    {
        InsetsF insets = InsetsF.VH(1.25f, 2.5f);
        Assert.Equal(1.25f, insets.Top);
        Assert.Equal(2.5f, insets.Left);
        Assert.Equal(1.25f, insets.Bottom);
        Assert.Equal(2.5f, insets.Right);
    }

    [Fact]
    private static void TestSetTop()
    {
        InsetsF insets1 = new(1.5f);
        insets1.SetTop(2.75f);
        Assert.Equal(2.75f, insets1.Top);
        Assert.Equal(1.5f, insets1.Left);
        Assert.Equal(1.5f, insets1.Bottom);
        Assert.Equal(1.5f, insets1.Right);
        InsetsF insets2 = new(1.5f);
        insets2.SetTop(2.75f);
        Assert.Equal(insets1, insets2);
    }

    [Fact]
    private static void TestSetBottom()
    {
        InsetsF insets1 = new(1.5f);
        insets1.SetBottom(2.75f);
        Assert.Equal(1.5f, insets1.Top);
        Assert.Equal(1.5f, insets1.Left);
        Assert.Equal(2.75f, insets1.Bottom);
        Assert.Equal(1.5f, insets1.Right);
        InsetsF insets2 = new(1.5f);
        insets2.SetBottom(2.75f);
        Assert.Equal(insets1, insets2);
    }

    [Fact]
    private static void TestSetLeft()
    {
        InsetsF insets1 = new(1.5f);
        insets1.SetLeft(2.75f);
        Assert.Equal(1.5f, insets1.Top);
        Assert.Equal(2.75f, insets1.Left);
        Assert.Equal(1.5f, insets1.Bottom);
        Assert.Equal(1.5f, insets1.Right);
        InsetsF insets2 = new(1.5f);
        insets2.SetLeft(2.75f);
        Assert.Equal(insets1, insets2);
    }

    [Fact]
    private static void TestSetRight()
    {
        InsetsF insets1 = new(1.5f);
        insets1.SetRight(2.75f);
        Assert.Equal(1.5f, insets1.Top);
        Assert.Equal(1.5f, insets1.Left);
        Assert.Equal(1.5f, insets1.Bottom);
        Assert.Equal(2.75f, insets1.Right);
        InsetsF insets2 =  new(1.5f);
        insets2.SetRight(2.75f);
        Assert.Equal(insets1, insets2);
    }

    [Fact]
    private static void TestWidthHeightAndIsEmpty()
    {
        InsetsF insets = new();
        Assert.Equal(0, insets.Width, 1e-6);
        Assert.Equal(0, insets.Height, 1e-6);
        Assert.True(insets.IsEmpty);

        insets.SetLeft(3.5f);
        insets.SetRight(4.25f);
        Assert.Equal(7.75f, insets.Width, 1e-6);
        Assert.Equal(0, insets.Height, 1e-6);
        Assert.False(insets.IsEmpty);

        insets.SetLeft(0);
        insets.SetRight(0);
        insets.SetTop(1.5f);
        insets.SetBottom(2.75f);
        Assert.Equal(0, insets.Width, 1e-6);
        Assert.Equal(4.25f, insets.Height, 1e-6);
        Assert.False(insets.IsEmpty);

        insets.SetLeft(4.25f);
        insets.SetRight(5);
        Assert.Equal(9.25f, insets.Width, 1e-6);
        Assert.Equal(4.25f, insets.Height, 1e-6);
        Assert.False(insets.IsEmpty);
    }

    [Fact]
    private static void TestTranspose()
    {
        InsetsF insets = InsetsF.TLBR(1.25f, 2.5f, 3.75f, 4.875f);
        insets.Transpose();
        Assert.Equal(insets, InsetsF.TLBR(2.5f, 1.25f, 4.875f, 3.75f));

        insets = new InsetsF();
        insets.Transpose();
        Assert.Equal(insets, new InsetsF());
    }

    [Fact]
    private static void TestOperators()
    {
        InsetsF insets = new();
        insets.SetLeft(2.5f);
        insets.SetRight(4.1f);
        insets.SetTop(1.0f);
        insets.SetBottom(3.3f);

        InsetsF insets2 = new();

        insets2.SetLeft(6.7f);
        insets2.SetRight(8.5f);
        insets2.SetTop(5.8f);
        insets2.SetBottom(7.6f);

        insets += insets2;

        Assert.Equal(6.8f, insets.Top, 1e-6);
        Assert.Equal(9.2f, insets.Left, 1e-6);
        Assert.Equal(10.9f, insets.Bottom, 1e-6);
        Assert.Equal(12.6f, insets.Right, 1e-6);
        
        insets2 = new();
        insets2.SetLeft(0);
        insets2.SetRight(2.2f);
        insets2.SetTop(-1.0f);
        insets2.SetBottom(1.1f);

        insets -= insets2;

        Assert.Equal(7.8f, insets.Top, 1e-6);
        Assert.Equal(9.2f, insets.Left, 1e-6);
        Assert.Equal(9.8f, insets.Bottom, 1e-6);
        Assert.Equal(10.4f, insets.Right, 1e-6);

        insets = new();
        insets.SetLeft(10.1f);
        insets.SetRight(10.001f);
        insets.SetTop(10);
        insets.SetBottom(10.01f);

        insets2 = new();
        insets2.SetLeft(5.0f);
        insets2.SetRight(-20.2f);
        insets2.SetTop(5.5f);
        insets2.SetBottom(0);

        insets = insets + insets2;

        Assert.Equal(15.5f, insets.Top, 1e-6);
        Assert.Equal(15.1f, insets.Left, 1e-6);
        Assert.Equal(10.01f, insets.Bottom, 1e-6);
        Assert.Equal(-10.199f, insets.Right, 1e-6);

        insets = new();
        insets.SetLeft(10.1f);
        insets.SetRight(10.001f);
        insets.SetTop(10);
        insets.SetBottom(10.01f);
            
        insets2 = new();
        insets2.SetLeft(5.0f);
        insets2.SetRight(-20.2f);
        insets2.SetTop(5.5f);
        insets2.SetBottom(0);

        insets = insets - insets2;
        Assert.Equal(4.5f, insets.Top, 1e-6);
        Assert.Equal(5.1f, insets.Left, 1e-6);
        Assert.Equal(10.01f, insets.Bottom, 1e-6);
        Assert.Equal(30.201f, insets.Right, 1e-6);
    }

    [Fact]
    private static void TestEquality()
    {
        InsetsF insets1 = new InsetsF();
        insets1.SetLeft(2.2f);
        insets1.SetRight(4.4f);
        insets1.SetTop(1.1f);
        insets1.SetBottom(3.3f);

        InsetsF insets2 = new();
        // Test operator== and operator!=.
        Assert.False(insets1 == insets2);
        Assert.True(insets1 != insets2);

        insets2.SetLeft(2.2f);
        insets2.SetRight(4.4f);
        insets2.SetTop(1.1f);
        insets2.SetBottom(3.3f);
        
        Assert.True(insets1 == insets2);
        Assert.False(insets1 != insets2);
    }

    [Fact]
    private static void TestToString()
    {
        InsetsF insets = new InsetsF();
        insets.SetLeft(2.2f);
        insets.SetRight(4.4f);
        insets.SetTop(1.1f);
        insets.SetBottom(3.3f);

        Assert.Equal("x:2.2,4.4 y:1.1,3.3", insets.ToString());
    }

    [Fact]
    private static void TestScale()
    {
        InsetsF inset = new InsetsF();
        inset.SetLeft(5);
        inset.SetRight(1);
        inset.SetTop(7);
        inset.SetBottom(3);

        InsetsF inset2 = InsetsF.ScaleInsets(inset, 2.5f, 3.5f);

        InsetsF expected = new();
        expected.SetLeft(12.5f);
        expected.SetRight(2.5f);
        expected.SetTop(24.5f);
        expected.SetBottom(10.5f);

        Assert.Equal(expected, inset2);

        inset2 = InsetsF.ScaleInsets(inset, 2.5f);

        expected = new InsetsF();
        expected.SetLeft(12.5f);
        expected.SetRight(2.5f);
        expected.SetTop(17.5f);
        expected.SetBottom(7.5f);

        Assert.Equal(expected, inset2);

        inset.Scale(2.5f, 3.5f);

        expected = new InsetsF();
        expected.SetLeft(12.5f);
        expected.SetRight(2.5f);
        expected.SetTop(24.5f);
        expected.SetBottom(10.5f);

        Assert.Equal(expected, inset);

        inset.Scale(-2.5f);

        expected = new InsetsF();
        expected.SetLeft(-31.25f);
        expected.SetRight(-6.25f);
        expected.SetTop(-61.25f);
        expected.SetBottom(-26.25f);

        Assert.Equal(expected, inset);
    }

    [Fact]
    private static void TestScaleNegative()
    {
        InsetsF inset = new InsetsF();
        inset.SetLeft(-5);
        inset.SetRight(-1);
        inset.SetTop(-7);
        inset.SetBottom(-3);

        InsetsF inset2 = InsetsF.ScaleInsets(inset, 2.5f, 3.5f);

        InsetsF expected = new InsetsF();
        expected.SetLeft(-12.5f);
        expected.SetRight(-2.5f);
        expected.SetTop(-24.5f);
        expected.SetBottom(-10.5f);

        Assert.Equal(expected, inset2);

        inset2 = InsetsF.ScaleInsets(inset, 2.5f);

        expected = new InsetsF();
        expected.SetLeft(-12.5f);
        expected.SetRight(-2.5f);
        expected.SetTop(-17.5f);
        expected.SetBottom(-7.5f);

        Assert.Equal(expected, inset2);

        inset.Scale(2.5f, 3.5f);

        expected = new InsetsF();
        expected.SetLeft(-12.5f);
        expected.SetRight(-2.5f);
        expected.SetTop(-24.5f);
        expected.SetBottom(-10.5f);

        Assert.Equal(expected, inset);

        inset.Scale(-2.5f);

        expected = new InsetsF();
        expected.SetLeft(31.25f);
        expected.SetRight(6.25f);
        expected.SetTop(61.25f);
        expected.SetBottom(26.25f);

        Assert.Equal(expected, inset);
    }

    [Fact]
    private static void TestSetToMax()
    {
        
    }

    [Fact]
    private static void TestConversionFromToOutsetsF()
    {
        
    }
}
