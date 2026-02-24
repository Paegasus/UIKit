using System.Runtime.InteropServices;
using UI.Numerics;

namespace UI.GFX.Geometry;

[StructLayout(LayoutKind.Sequential)]
public struct LineF
{
    public PointF p1;
    public PointF p2;

    public readonly Vector2DF Normal()
    {
        return new Vector2DF(p1.Y - p2.Y, p2.X - p1.X);
    }

    public readonly PointF? IntersectionWith(in LineF other)
    {
        Vector2DF a_length = p2 - p1;
        Vector2DF b_length = other.p2 - other.p1;

        float denom = (float)Vector2DF.CrossProduct(a_length, b_length);

        if (denom == 0)
        {
            return null;
        }

        float param = (float)Vector2DF.CrossProduct(other.p1 - p1, b_length) / denom;
        
        return p1 + Vector2DF.ScaleVector2D(a_length, param);
    }
}
