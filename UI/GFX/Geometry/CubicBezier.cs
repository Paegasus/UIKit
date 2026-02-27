using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UI.GFX.Geometry;

public struct CubicBezier
{
    [InlineArray(11)]
    private struct SplineSamples
    {
        private double _element0;
    }

    private SplineSamples spline_samples_;

    private double ax_;
    private double bx_;
    private double cx_;

    private double ay_;
    private double by_;
    private double cy_;

    private double start_gradient_;
    private double end_gradient_;

    private double range_min_;
    private double range_max_;

#if DEBUG
    // Guard against attempted to solve for t given x in the event
    // that the curve may have multiple values for t for some values of x in [0, 1].
    private bool monotonically_increasing_;
#endif

    private void InitCoefficients(double p1x, double p1y, double p2x, double p2y)
    {
        throw new NotImplementedException();
    }

    private void InitGradients(double p1x, double p1y, double p2x, double p2y)
    {
        throw new NotImplementedException();
    }

    private void InitRange(double p1y, double p2y)
    {
        throw new NotImplementedException();
    }

    private void InitSpline()
    {
        throw new NotImplementedException();
    }

    private static double ToFinite(double value)
    {
        throw new NotImplementedException();
    }

    public CubicBezier(double p1x, double p1y, double p2x, double p2y)
    {
        throw new NotImplementedException();
    }

    public CubicBezier(in CubicBezier other)
    {
        throw new NotImplementedException();
    }

    public readonly double SampleCurveX(double t)
    {
        // `ax t^3 + bx t^2 + cx t' expanded using Horner's rule.
        // The x values are in the range [0, 1].
        // So it isn't needed toFinite clamping.
        // https://drafts.csswg.org/css-easing-1/#funcdef-cubic-bezier-easing-function-cubic-bezier
        return ((ax_ * t + bx_) * t + cx_) * t;
    }

    public readonly double SampleCurveY(double t)
    {
        return ToFinite(((ay_ * t + by_) * t + cy_) * t);
    }

    public readonly double SampleCurveDerivativeX(double t)
    {
        return (3.0 * ax_ * t + 2.0 * bx_) * t + cx_;
    }

    public readonly double SampleCurveDerivativeY(double t)
    {
        return ToFinite(
            ToFinite(ToFinite(3.0 * ay_) * t + ToFinite(2.0 * by_)) * t + cy_);
    }

    public static double GetDefaultEpsilon()
    {
        throw new NotImplementedException();
    }

    // Given an x value, find a parametric value it came from.
    // x must be in [0, 1] range. Doesn't use gradients.
    public readonly double SolveCurveX(double x, double epsilon)
    {
        throw new NotImplementedException();
    }

    // Evaluates y at the given x with default epsilon.
    public readonly double Solve(double x)
    {
        throw new NotImplementedException();
    }

    // Evaluates y at the given x. The epsilon parameter provides a hint as to the
    // required accuracy and is not guaranteed. Uses gradients if x is
    // out of [0, 1] range.
    public readonly double SolveWithEpsilon(double x, double epsilon)
    {
        if (x < 0.0)
            return ToFinite(0.0 + start_gradient_ * x);
        if (x > 1.0)
            return ToFinite(1.0 + end_gradient_ * (x - 1.0));
        return SampleCurveY(SolveCurveX(x, epsilon));
    }

    // Returns an approximation of dy/dx at the given x with default epsilon.
    public readonly double Slope(double x)
    {
        throw new NotImplementedException();
    }

    // Returns an approximation of dy/dx at the given x.
    // Clamps x to range [0, 1].
    public readonly double SlopeWithEpsilon(double x, double epsilon)
    {
        throw new NotImplementedException();
    }

    // These getters are used rarely. We reverse compute them from coefficients.
    // See CubicBezier::InitCoefficients. The speed has been traded for memory.
    public readonly double GetX1()
    {
        throw new NotImplementedException();
    }
    
    public readonly double GetY1()
    {
        throw new NotImplementedException();
    }

    public readonly double GetX2()
    {
        throw new NotImplementedException();
    }

    public readonly double GetY2()
    {
        throw new NotImplementedException();
    }

    // Gets the bezier's minimum y value in the interval [0, 1].
    public readonly double range_min()
    {
        return range_min_;
    }

    // Gets the bezier's maximum y value in the interval [0, 1].
    public readonly double range_max()
    {
        return range_max_;
    }
}
