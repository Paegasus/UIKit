using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UI.GFX.Geometry;

public struct CubicBezier
{
    private static int CUBIC_BEZIER_SPLINE_SAMPLES = 11;
    private static int kMaxNewtonIterations = 4;
    private static readonly double kBezierEpsilon = 1e-7;

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

    public CubicBezier(double p1x, double p1y, double p2x, double p2y)
    {
        InitCoefficients(p1x, p1y, p2x, p2y);
        InitGradients(p1x, p1y, p2x, p2y);
        InitRange(p1y, p2y);
        InitSpline();
    }

    private void InitCoefficients(double p1x, double p1y, double p2x, double p2y)
    {
        // Calculate the polynomial coefficients, implicit first and last control
        // points are (0,0) and (1,1).
        cx_ = 3.0 * p1x;
        bx_ = 3.0 * (p2x - p1x) - cx_;
        ax_ = 1.0 - cx_ - bx_;

        cy_ = ToFinite(3.0 * p1y);
        by_ = ToFinite(3.0 * (p2y - p1y) - cy_);
        ay_ = ToFinite(1.0 - cy_ - by_);

#if DEBUG
        // Bezier curves with x-coordinates outside the range [0,1] for internal
        // control points may have multiple values for t for a given value of x.
        // In this case, calls to SolveCurveX may produce ambiguous results.
        monotonically_increasing_ = p1x >= 0 && p1x <= 1 && p2x >= 0 && p2x <= 1;
#endif
    }

    private void InitGradients(double p1x, double p1y, double p2x, double p2y)
    {
        // End-point gradients are used to calculate timing function results
        // outside the range [0, 1].
        //
        // There are four possibilities for the gradient at each end:
        // (1) the closest control point is not horizontally coincident with regard to
        //     (0, 0) or (1, 1). In this case the line between the end point and
        //     the control point is tangent to the bezier at the end point.
        // (2) the closest control point is coincident with the end point. In
        //     this case the line between the end point and the far control
        //     point is tangent to the bezier at the end point.
        // (3) both internal control points are coincident with an endpoint. There
        //     are two special case that fall into this category:
        //     CubicBezier(0, 0, 0, 0) and CubicBezier(1, 1, 1, 1). Both are
        //     equivalent to linear.
        // (4) the closest control point is horizontally coincident with the end
        //     point, but vertically distinct. In this case the gradient at the
        //     end point is Infinite. However, this causes issues when
        //     interpolating. As a result, we break down to a simple case of
        //     0 gradient under these conditions.

        if (p1x > 0)
            start_gradient_ = p1y / p1x;
        else if (p1y == 0 && p2x > 0)
            start_gradient_ = p2y / p2x;
        else if (p1y == 0 && p2y == 0)
            start_gradient_ = 1;
        else
            start_gradient_ = 0;

        if (p2x < 1)
            end_gradient_ = (p2y - 1) / (p2x - 1);
        else if (p2y == 1 && p1x < 1)
            end_gradient_ = (p1y - 1) / (p1x - 1);
        else if (p2y == 1 && p1y == 1)
            end_gradient_ = 1;
        else
            end_gradient_ = 0;
    }

    // This works by taking taking the derivative of the cubic bezier, on the y axis.
    // We can then solve for where the derivative is zero
    // to find the min and max distance along the line.
    // We the have to solve those in terms of time
    // rather than distance on the x-axis
    private void InitRange(double p1y, double p2y)
    {
        range_min_ = 0;
        range_max_ = 1;
        if (0 <= p1y && p1y < 1 && 0 <= p2y && p2y <= 1)
            return;

        double epsilon = kBezierEpsilon;

        // Represent the function's derivative in the form at^2 + bt + c
        // as in sampleCurveDerivativeY.
        // (Technically this is (dy/dt)*(1/3), which is suitable for finding zeros
        // but does not actually give the slope of the curve.)
        double a = 3.0 * ay_;
        double b = 2.0 * by_;
        double c = cy_;

        // Check if the derivative is constant.
        if (Math.Abs(a) < epsilon && Math.Abs(b) < epsilon)
            return;

        // Zeros of the function's derivative.
        double t1 = 0;
        double t2 = 0;

        if (Math.Abs(a) < epsilon)
        {
            // The function's derivative is linear.
            t1 = -c / b;
        }
        else
        {
            // The function's derivative is a quadratic. We find the zeros of this
            // quadratic using the quadratic formula.
            double discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
                return;
            double discriminant_sqrt = Math.Sqrt(discriminant);
            t1 = (-b + discriminant_sqrt) / (2 * a);
            t2 = (-b - discriminant_sqrt) / (2 * a);
        }

        double sol1 = 0;
        double sol2 = 0;

        // If the solution is in the range [0,1] then we include it, otherwise we
        // ignore it.

        // An interesting fact about these beziers is that they are only
        // actually evaluated in [0,1]. After that we take the tangent at that point
        // and linearly project it out.
        if (0 < t1 && t1 < 1)
            sol1 = SampleCurveY(t1);

        if (0 < t2 && t2 < 1)
            sol2 = SampleCurveY(t2);

        range_min_ = Math.Min(range_min_, Math.Min(sol1, sol2));
        range_max_ = Math.Max(range_max_, Math.Min(sol1, sol2));
    }

    private void InitSpline()
    {
        double delta_t = 1.0 / (CUBIC_BEZIER_SPLINE_SAMPLES - 1);
        for (int i = 0; i < CUBIC_BEZIER_SPLINE_SAMPLES; i++)
        {
            spline_samples_[i] = SampleCurveX(i * delta_t);
        }
    }

    private static double ToFinite(double value)
    {
        // We can clamp this in numeric operation helper function.
        if (double.IsInfinity(value))
        {
            if (value > 0)
                return double.MaxValue;
            return double.MinValue;
        }

        return value;
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
        return ToFinite(ToFinite(ToFinite(3.0 * ay_) * t + ToFinite(2.0 * by_)) * t + cy_);
    }

    public static double GetDefaultEpsilon() => kBezierEpsilon;

    // Given an x value, find a parametric value it came from.
    // x must be in [0, 1] range. Doesn't use gradients.
    public readonly double SolveCurveX(double x, double epsilon)
    {
#if DEBUG
        Debug.Assert(x >= 0.0);
        Debug.Assert(x <= 1.0);
#endif

        double t0 = 0;
        double t1 = 0;
        double t2 = x;
        double x2 = 0;
        double d2 = 0;
        int i = 0;

#if DEBUG
        Debug.Assert(monotonically_increasing_);
#endif

        // Linear interpolation of spline curve for initial guess.
        double delta_t = 1.0 / (CUBIC_BEZIER_SPLINE_SAMPLES - 1);
        for (i = 1; i < CUBIC_BEZIER_SPLINE_SAMPLES; i++)
        {
            if (x <= spline_samples_[i])
            {
                t1 = delta_t * i;
                t0 = t1 - delta_t;
                t2 = t0 + (t1 - t0) * (x - spline_samples_[i - 1]) /
                              (spline_samples_[i] - spline_samples_[i - 1]);
                break;
            }
        }

        // Perform a few iterations of Newton's method -- normally very fast.
        // See https://en.wikipedia.org/wiki/Newton%27s_method.
        double newton_epsilon = Math.Min(kBezierEpsilon, epsilon);
        for (i = 0; i < kMaxNewtonIterations; i++)
        {
            x2 = SampleCurveX(t2) - x;
            if (Math.Abs(x2) < newton_epsilon)
                return t2;
            d2 = SampleCurveDerivativeX(t2);
            if (Math.Abs(d2) < kBezierEpsilon)
                break;
            t2 = t2 - x2 / d2;
        }
        if (Math.Abs(x2) < epsilon)
            return t2;

        // Fall back to the bisection method for reliability.
        while (t0 < t1)
        {
            x2 = SampleCurveX(t2);
            if (Math.Abs(x2 - x) < epsilon)
                return t2;
            if (x > x2)
                t0 = t2;
            else
                t1 = t2;
            t2 = (t1 + t0) * .5;
        }

        // Failure.
        return t2;
    }

    // Evaluates y at the given x with default epsilon.
    public readonly double Solve(double x) => SolveWithEpsilon(x, kBezierEpsilon);

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
    public readonly double Slope(double x) => SlopeWithEpsilon(x, kBezierEpsilon);

    // Returns an approximation of dy/dx at the given x.
    // Clamps x to range [0, 1].
    public readonly double SlopeWithEpsilon(double x, double epsilon)
    {
        x = Math.Clamp(x, 0.0, 1.0);
        double t = SolveCurveX(x, epsilon);
        double dx = SampleCurveDerivativeX(t);
        double dy = SampleCurveDerivativeY(t);
        // We should clamp NaN to a proper value.
        if (dx == 0 && dy == 0)
            return 0;
        return ToFinite(dy / dx);
    }

    // These getters are used rarely. We reverse compute them from coefficients.
    // See CubicBezier::InitCoefficients. The speed has been traded for memory.
    public readonly double GetX1() => cx_ / 3.0;

    public readonly double GetY1() => cy_ / 3.0;

    public readonly double GetX2() => (bx_ + cx_) / 3.0 + GetX1();

    public readonly double GetY2() => (by_ + cy_) / 3.0 + GetY1();

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
