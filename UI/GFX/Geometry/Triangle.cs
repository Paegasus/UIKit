
namespace UI.GFX.Geometry;

public static class Triangle
{
    public static bool PointIsInTriangle(in PointF point, in PointF r1, in PointF r2, in PointF r3)
    {
        // Compute the barycentric coordinates (u, v, w) of |point| relative to the
        // triangle (r1, r2, r3) by the solving the system of equations:
        //   1) point = u * r1 + v * r2 + w * r3
        //   2) u + v + w = 1
        // This algorithm comes from Christer Ericson's Real-Time Collision Detection.

        Vector2DF r31 = r1 - r3;
        Vector2DF r32 = r2 - r3;
        Vector2DF r3p = point - r3;

        // Promote to doubles so all the math below is done with doubles, because
        // otherwise it gets incorrect results on arm64.
        double r31x = r31.X;
        double r31y = r31.Y;
        double r32x = r32.X;
        double r32y = r32.Y;

        double denom = r32y * r31x - r32x * r31y;
        double u = (r32y * r3p.X - r32x * r3p.Y) / denom;
        double v = (r31x * r3p.Y - r31y * r3p.X) / denom;
        double w = 1.0 - u - v;

        // Use the barycentric coordinates to test if |point| is inside the
        // triangle (r1, r2, r2).
        return (u >= 0) && (v >= 0) && (w >= 0);
    }
}
