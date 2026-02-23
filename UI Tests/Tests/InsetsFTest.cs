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
        InsetsF insets = new InsetsF(1.5f);
        insets.SetTop(2.75f);
        Assert.Equal(2.75f, insets.Top);
        Assert.Equal(1.5f, insets.Left);
        Assert.Equal(1.5f, insets.Bottom);
        Assert.Equal(1.5f, insets.Right);
        Assert.Equal(insets, new InsetsF(1.5f).SetTop(2.75f));
    }

    [Fact]
    private static void TestSetBottom()
    {
        InsetsF insets = new(1.5f);
        insets.SetBottom(2.75f);
        Assert.Equal(1.5f, insets.Top);
        Assert.Equal(1.5f, insets.Left);
        Assert.Equal(2.75f, insets.Bottom);
        Assert.Equal(1.5f, insets.Right);
        Assert.Equal(insets, new InsetsF(1.5f).SetBottom(2.75f));
    }

    [Fact]
    private static void TestSetLeft()
    {
        InsetsF insets = new(1.5f);
        insets.SetLeft(2.75f);
        Assert.Equal(1.5f, insets.Top);
        Assert.Equal(2.75f, insets.Left);
        Assert.Equal(1.5f, insets.Bottom);
        Assert.Equal(1.5f, insets.Right);
        Assert.Equal(insets, new InsetsF(1.5f).SetLeft(2.75f));
    }

    [Fact]
    private static void TestSetRight()
    {
        InsetsF insets = new(1.5f);
        insets.SetRight(2.75f);
        Assert.Equal(1.5f, insets.Top);
        Assert.Equal(1.5f, insets.Left);
        Assert.Equal(1.5f, insets.Bottom);
        Assert.Equal(2.75f, insets.Right);
        Assert.Equal(insets, new InsetsF(1.5f).SetRight(2.75f));
    }
}
