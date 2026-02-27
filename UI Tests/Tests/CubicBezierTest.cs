using Xunit;

using UI.GFX.Geometry;

namespace UI.Tests;

public static class CubicBezierTest
{
    [Fact]
    private static void TestBasic()
    {
        CubicBezier function = new(0.25, 0.0, 0.75, 1.0);

        double epsilon = 0.00015;

        Assert.Equal(0, function.Solve(0), epsilon);
        Assert.Equal(0.01136, function.Solve(0.05), epsilon);
        Assert.Equal(0.03978, function.Solve(0.1), epsilon);
        Assert.Equal(0.079780, function.Solve(0.15), epsilon);
        Assert.Equal(0.12803, function.Solve(0.2), epsilon);
        Assert.Equal(0.18235, function.Solve(0.25), epsilon);
        Assert.Equal(0.24115, function.Solve(0.3), epsilon);
        Assert.Equal(0.30323, function.Solve(0.35), epsilon);
        Assert.Equal(0.36761, function.Solve(0.4), epsilon);
        Assert.Equal(0.43345, function.Solve(0.45), epsilon);
        Assert.Equal(0.5, function.Solve(0.5), epsilon);
        Assert.Equal(0.63238, function.Solve(0.6), epsilon);
        Assert.Equal(0.69676, function.Solve(0.65), epsilon);
        Assert.Equal(0.75884, function.Solve(0.7), epsilon);
        Assert.Equal(0.81764, function.Solve(0.75), epsilon);
        Assert.Equal(0.87196, function.Solve(0.8), epsilon);
        Assert.Equal(0.92021, function.Solve(0.85), epsilon);
        Assert.Equal(0.96021, function.Solve(0.9), epsilon);
        Assert.Equal(0.98863, function.Solve(0.95), epsilon);
        Assert.Equal(1, function.Solve(1), epsilon);

        CubicBezier basic_use = new(0.5, 1.0, 0.5, 1.0);
        Assert.Equal(0.875, basic_use.Solve(0.5), 1e-7);

        CubicBezier overshoot = new(0.5, 2.0, 0.5, 2.0);
        Assert.Equal(1.625, overshoot.Solve(0.5), 1e-7);

        CubicBezier undershoot = new(0.5, -1.0, 0.5, -1.0);
        Assert.Equal(-0.625, undershoot.Solve(0.5), 1e-7);
    }
}
