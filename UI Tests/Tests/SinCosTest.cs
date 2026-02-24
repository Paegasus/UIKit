using Xunit;

using UI.GFX.Geometry;

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
        for (int d = -3600; d <= 3600; ++d)
        {
            double degrees = d * 0.1;
            Assert.Equal(Math.Sin(degrees * Math.PI / 180.0), SinCos.SinCosDegrees(degrees).sin, 1e-6);
            Assert.Equal(Math.Cos(degrees * Math.PI / 180.0), SinCos.SinCosDegrees(degrees).cos, 1e-6);
        }
    }

    [Fact]
    private static void TestAccurateRangeReduction()
    {
        Assert.Equal(SinCos.SinCosDegrees(90000123).sin, SinCos.SinCosDegrees(90000123).sin);
        Assert.Equal(SinCos.SinCosDegrees(90000123).cos, SinCos.SinCosDegrees(90000123).cos);

        Assert.Equal(0.0, SinCos.SinCosDegrees(90e5).sin);
        Assert.Equal(1.0, SinCos.SinCosDegrees(90e5).cos);
    }

    [Fact]
    private static void TestHugeValues()
    {
        Assert.Equal(SinCos.SinCosDegrees(360e10 + 20).sin, Math.Sin(20 * (Math.PI / 180.0)), 1e-6);
        Assert.Equal(SinCos.SinCosDegrees(360e10 + 20).cos, Math.Cos(20 * (Math.PI / 180.0)), 1e-6);
    }
}
