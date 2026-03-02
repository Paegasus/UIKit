using System.Diagnostics;

namespace UI.GFX.Geometry;

public struct ThreePointCubicBezier
{
    private CubicBezier first_curve_;
    private CubicBezier second_curve_;

    private double midpointx_;
    private double midpointy_;

    // Creates a curve composed of two cubic curves sharing a common midpoint. The
    // curve runs through the (0,0), the midpoint, and (1,1). |p1x|, |p1y|, |p2x|,
    // and |p2y| are the coordinates of the control points for the first curve and
    // |p3x|, |p3y|, |p4x|, and |p4y| are the coordinates of the control points
    // for the second curve.
    public ThreePointCubicBezier(double p1x,
                          double p1y,
                          double p2x,
                          double p2y,
                          double midpointx,
                          double midpointy,
                          double p3x,
                          double p3y,
                          double p4x,
                          double p4y)
    {
        first_curve_ = new(p1x / midpointx,
                           p1y / midpointy,
                           p2x / midpointx,
                           p2y / midpointy);

        second_curve_ = new((p3x - midpointx) / (1 - midpointx),
                            (p3y - midpointy) / (1 - midpointy),
                            (p4x - midpointx) / (1 - midpointx),
                            (p4y - midpointy) / (1 - midpointy));

        midpointx_ = midpointx;
        midpointy_ = midpointy;
    }

    // Evaluates y at the given x.
    public readonly double Solve(double x)
    {
        bool in_first_curve = x < midpointx_;
        double scaled_x = (x - (in_first_curve ? 0.0 : midpointx_)) /
                                  (in_first_curve ? midpointx_ : (1 - midpointx_));
        if (in_first_curve)
        {
            return first_curve_.Solve(scaled_x) * midpointy_;
        }
        return second_curve_.Solve(scaled_x) * (1 - midpointy_) + midpointy_;
    }
}
