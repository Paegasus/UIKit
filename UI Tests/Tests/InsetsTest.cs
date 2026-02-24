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
}
