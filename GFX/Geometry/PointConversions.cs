using static UI.Numerics.SafeConversions;

namespace UI.GFX.Geometry;

public static class PointConversions
{
    public static Point ToFlooredPoint(in PointF point)
    {
        return new Point(ClampFloor(point.X), ClampFloor(point.Y));
    }

    public static Point ToCeiledPoint(in PointF point)
    {
        return new Point(ClampCeil(point.X), ClampCeil(point.Y));
    }

    public static Point ToRoundedPoint(in PointF point)
    {
        return new Point(ClampRound(point.X), ClampRound(point.Y));
    }
}
