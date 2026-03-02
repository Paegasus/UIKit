using Xunit;

using UI.GFX.Geometry;

namespace UI.Tests;

public static class ThreePointCubicBezierTest
{
    [Fact]
    private static void TestBasic()
    {
        ThreePointCubicBezier function = new(0.125f, 0.0f, 0.375f, 0.5f, 0.5f, 0.5f, 0.625f, 0.5f, 0.875f, 1f);

        double epsilon = 0.00015;

        Assert.Equal(0, function.Solve(0), epsilon);
        Assert.Equal(0.01989, function.Solve(0.05), epsilon);
        Assert.Equal(0.06402, function.Solve(0.1), epsilon);
        Assert.Equal(0.12058, function.Solve(0.15), epsilon);
        Assert.Equal(0.18381, function.Solve(0.2), epsilon);
        Assert.Equal(0.25, function.Solve(0.25), epsilon);
        Assert.Equal(0.31619, function.Solve(0.3), epsilon);
        Assert.Equal(0.37942, function.Solve(0.35), epsilon);
        Assert.Equal(0.43598, function.Solve(0.4), epsilon);
        Assert.Equal(0.48011, function.Solve(0.45), epsilon);
        Assert.Equal(0.5, function.Solve(0.5), epsilon);
        Assert.Equal(0.51989, function.Solve(0.55), epsilon);
        Assert.Equal(0.56402, function.Solve(0.6), epsilon);
        Assert.Equal(0.62058, function.Solve(0.65), epsilon);
        Assert.Equal(0.68381, function.Solve(0.7), epsilon);
        Assert.Equal(0.75, function.Solve(0.75), epsilon);
        Assert.Equal(0.81619, function.Solve(0.8), epsilon);
        Assert.Equal(0.87942, function.Solve(0.85), epsilon);
        Assert.Equal(0.93598, function.Solve(0.9), epsilon);
        Assert.Equal(0.98001, function.Solve(0.95), epsilon);
        Assert.Equal(1, function.Solve(1), epsilon);
    }
}
