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

    // Tests that solving the bezier works with knots with y not in (0, 1).
    [Fact]
    private static void TestUnclampedYValues()
    {
        CubicBezier function = new(0.5, -1.0, 0.5, 2.0);

        double epsilon = 0.00015;

        Assert.Equal(0.0, function.Solve(0.0), epsilon);
        Assert.Equal(-0.08954, function.Solve(0.05), epsilon);
        Assert.Equal(-0.15613, function.Solve(0.1), epsilon);
        Assert.Equal(-0.19641, function.Solve(0.15), epsilon);
        Assert.Equal(-0.20651, function.Solve(0.2), epsilon);
        Assert.Equal(-0.18232, function.Solve(0.25), epsilon);
        Assert.Equal(-0.11992, function.Solve(0.3), epsilon);
        Assert.Equal(-0.01672, function.Solve(0.35), epsilon);
        Assert.Equal(0.12660, function.Solve(0.4), epsilon);
        Assert.Equal(0.30349, function.Solve(0.45), epsilon);
        Assert.Equal(0.50000, function.Solve(0.5), epsilon);
        Assert.Equal(0.69651, function.Solve(0.55), epsilon);
        Assert.Equal(0.87340, function.Solve(0.6), epsilon);
        Assert.Equal(1.01672, function.Solve(0.65), epsilon);
        Assert.Equal(1.11992, function.Solve(0.7), epsilon);
        Assert.Equal(1.18232, function.Solve(0.75), epsilon);
        Assert.Equal(1.20651, function.Solve(0.8), epsilon);
        Assert.Equal(1.19641, function.Solve(0.85), epsilon);
        Assert.Equal(1.15613, function.Solve(0.9), epsilon);
        Assert.Equal(1.08954, function.Solve(0.95), epsilon);
        Assert.Equal(1.0, function.Solve(1.0), epsilon);
    }

    private static void TestBezierFiniteRange(in CubicBezier function)
    {
        for (double i = 0; i <= 1.01; i += 0.05)
        {
            Assert.True(double.IsFinite(function.Solve(i)));
            Assert.True(double.IsFinite(function.Slope(i)));
            Assert.True(double.IsFinite(function.GetX2()));
            Assert.True(double.IsFinite(function.GetY2()));
            Assert.True(double.IsFinite(function.SampleCurveX(i)));
            Assert.True(double.IsFinite(function.SampleCurveY(i)));
            Assert.True(double.IsFinite(function.SampleCurveDerivativeX(i)));
            Assert.True(double.IsFinite(function.SampleCurveDerivativeY(i)));
        }
    }

    // Tests that solving the bezier works with huge value infinity evaluation
    [Fact]
    private static void TestClampInfinityEvaluation()
    {
        CubicBezier[] test_cases =
        [
            new CubicBezier(0.5, double.MaxValue, 0.5, double.MaxValue),
            new CubicBezier(0.5, double.MinValue, 0.5, double.MaxValue),
			new CubicBezier(0.5, double.MaxValue, 0.5, double.MinValue),
			new CubicBezier(0.5, double.MinValue, 0.5, double.MinValue),

			new CubicBezier(0, double.MaxValue, 0, double.MaxValue),
			new CubicBezier(0, double.MinValue, 0, double.MaxValue),
			new CubicBezier(0, double.MaxValue, 0, double.MinValue),
			new CubicBezier(0, double.MinValue, 0, double.MinValue),

			new CubicBezier(1, double.MaxValue, 1, double.MaxValue),
			new CubicBezier(1, double.MinValue, 1, double.MaxValue),
			new CubicBezier(1, double.MaxValue, 1, double.MinValue),
			new CubicBezier(1, double.MinValue, 1, double.MinValue),

			new CubicBezier(0, 0, 0, double.MaxValue),
			new CubicBezier(0, double.MinValue, 0, 0),
			new CubicBezier(1, 0, 0, double.MinValue),
			new CubicBezier(0, double.MinValue, 1, 1),

        ];

        foreach (var tc in test_cases)
        {
            TestBezierFiniteRange(tc);
        }
    }

    [Fact]
    private static void TestRange()
    {
        double epsilon = 0.00015;
        // Derivative has two roots in [0,1].
        CubicBezier function = new(0.25, 2.5, 0.75, 0.5);
        Assert.Equal(0, function.range_min);
        Assert.Equal(1.28818, function.range_max, epsilon); // Failed, Expected: 1.28818, Actual: 1

        /*
        double epsilon = 0.00015;

        // Derivative is a constant.
        CubicBezier function = new(0.25, (1.0 / 3.0), 0.75, (2.0 / 3.0));
        Assert.Equal(0, function.range_min);
        Assert.Equal(1, function.range_max);

        // Derivative is linear.
        function = new(0.25, -0.5, 0.75, (-1.0 / 6.0));
        Assert.Equal(-0.225, function.range_min, epsilon);
        Assert.Equal(1, function.range_max);
        
          // Derivative has no real roots.
          function = new(0.25, 0.25, 0.75, 0.5);
          Assert.Equal(0, function.range_min);
          Assert.Equal(1, function.range_max);

          // Derivative has exactly one real root.
          function = new(0.0, 1.0, 1.0, 0.0);
          Assert.Equal(0, function.range_min);
          Assert.Equal(1, function.range_max);

          // Derivative has one root < 0 and one root > 1.
          function = new(0.25, 0.1, 0.75, 0.9);
          Assert.Equal(0, function.range_min);
          Assert.Equal(1, function.range_max);

          // Derivative has two roots in [0,1].
          function = new(0.25, 2.5, 0.75, 0.5);
          Assert.Equal(0, function.range_min);
          Assert.Equal(1.28818, function.range_max, epsilon);
          function = new(0.25, 0.5, 0.75, -1.5);
          Assert.Equal(-0.28818, function.range_min, epsilon);
          Assert.Equal(1, function.range_max);

          // Derivative has one root < 0 and one root in [0,1].
          function = new(0.25, 0.1, 0.75, 1.5);
          Assert.Equal(0, function.range_min);
          Assert.Equal(1.10755, function.range_max, epsilon);

          // Derivative has one root in [0,1] and one root > 1.
          function = new(0.25, -0.5, 0.75, 0.9);
          Assert.Equal(-0.10755, function.range_min, epsilon);
          Assert.Equal(1, function.range_max);

          // Derivative has two roots < 0.
          function = new(0.25, 0.3, 0.75, 0.633);
          Assert.Equal(0, function.range_min);
          Assert.Equal(1, function.range_max);

          // Derivative has two roots > 1.
          function = new(0.25, 0.367, 0.75, 0.7);
          Assert.Equal(0.0f, function.range_min);
          Assert.Equal(1.0f, function.range_max);
          */
    }
}
