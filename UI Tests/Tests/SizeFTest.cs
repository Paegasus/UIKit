using Xunit;

using UI.GFX.Geometry;

namespace UI.Tests;

public static class SizeFTest
{
    [Fact]
    private static void TestSizeToSizeF()
    {
        // Check that explicit conversion from integer to float compiles.
        Size a = new(10, 20);
        Assert.Equal(10, new SizeF(a).Width);
        Assert.Equal(20, new SizeF(a).Height);

        SizeF b = new(10, 20);
        Assert.Equal(b, new SizeF(a));
    }
}
