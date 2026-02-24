using Xunit;

using UI.GFX.Geometry;
using UI.Extensions;

namespace UI.Tests;

public static class SinCosTest
{
    [Fact]
    private static void TestExactValues()
    {
        for (int turn = -5 * 360; turn <= 5 * 360; turn += 360)
        {
            Assert.Equal(0.0, SinCos.SinCosDegrees(turn + 0).sin);
            Assert.Equal(1.0, SinCos.SinCosDegrees(turn + 0).cos);

            Assert.Equal(1.0, SinCos.SinCosDegrees(turn + 90).sin);
            Assert.Equal(0.0, SinCos.SinCosDegrees(turn + 90).cos);

            Assert.Equal(0.0, SinCos.SinCosDegrees(turn + 180).sin);
            Assert.Equal(-1.0, SinCos.SinCosDegrees(turn + 180).cos);

            Assert.Equal(-1.0, SinCos.SinCosDegrees(turn + 270).sin);
            Assert.Equal(0.0, SinCos.SinCosDegrees(turn + 270).cos);
        }
    }

    [Fact]
    private static void TestCloseToLibc()
    {
        
    }

    [Fact]
    private static void TestAccurateRangeReduction()
    {
        
    }
}
