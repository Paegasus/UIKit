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
}
