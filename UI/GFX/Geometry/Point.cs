namespace UI.GFX.Geometry;

using System.Runtime.CompilerServices;
using static Numerics.ClampedMath;

// A point has an x and y coordinate.
public struct Point : IComparable<Point>
{
    public int X;
    public int Y;

    public Point() => (X, Y) = (0, 0);
    
    public Point(int x, int y) => (X, Y) = (x, y);

    public void SetPoint(int x, int y) => (X, Y) = (x, y);

    public void Offset(int delta_x, int delta_y)
    {
        X = ClampAdd(X, delta_x);
        Y = ClampAdd(Y, delta_y);
    }

    public void SetToMin(in Point other)
    {
        X = Math.Min(X, other.X);
        Y = Math.Min(Y, other.Y);
    }

    public void SetToMax(in Point other)
    {
        X = Math.Max(X, other.X);
        Y = Math.Max(Y, other.Y);
    }

    public readonly bool IsOrigin() => X == 0 && Y == 0;

    public readonly Vector2D OffsetFromOrigin() => new(X, Y);

    public void Transpose() => (X, Y) = (Y, X); // Swap x_ and y_ (using tuple deconstruction swap)

    public readonly int CompareTo(Point other)
    {
        int yComparison = Y.CompareTo(other.Y);
        return yComparison != 0 ? yComparison : X.CompareTo(other.X);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point PointAtOffsetFromOrigin(in Vector2D offset_from_origin)
    {
        return new Point(offset_from_origin.X, offset_from_origin.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point TransposePoint(in Point p)
    {
        return new Point(p.Y, p.X);
    }

    // Helper methods to scale a Point to a new Point.

    public static Point ScaleToCeiledPoint(in Point point, float x_scale, float y_scale)
    {
        if (x_scale == 1.0f && y_scale == 1.0f)
            return point;
        return PointConversions.ToCeiledPoint(PointF.ScalePoint(new PointF(point), x_scale, y_scale));
    }

    public static Point ScaleToCeiledPoint(in Point point, float scale)
    {
        if (scale == 1.0f)
            return point;
        return PointConversions.ToCeiledPoint(PointF.ScalePoint(new PointF(point), scale, scale));
    }

    public static Point ScaleToFlooredPoint(in Point point, float x_scale, float y_scale)
    {
        if (x_scale == 1.0f && y_scale == 1.0f)
            return point;
        return PointConversions.ToFlooredPoint(PointF.ScalePoint(new PointF(point), x_scale, y_scale));
    }

    public static Point ScaleToFlooredPoint(in Point point, float scale)
    {
        if (scale == 1.0f)
            return point;
        return PointConversions.ToFlooredPoint(PointF.ScalePoint(new PointF(point), scale, scale));
    }

    public static Point ScaleToRoundedPoint(in Point point, float x_scale, float y_scale)
    {
        if (x_scale == 1.0f && y_scale == 1.0f)
            return point;
        return PointConversions.ToRoundedPoint(PointF.ScalePoint(new PointF(point), x_scale, y_scale));
    }

    public static Point ScaleToRoundedPoint(in Point point, float scale)
    {
        if (scale == 1.0f)
            return point;
        return PointConversions.ToRoundedPoint(PointF.ScalePoint(new PointF(point), scale, scale));
    }

    public override readonly string ToString() => $"{X},{Y}";

    // For use in collections (SortedSet, Dictionary keys, etc.)
    public override readonly int GetHashCode() => HashCode.Combine(Y, X);

    public readonly bool Equals(in Point other) => X == other.X && Y == other.Y;

    public override readonly bool Equals(object? obj) => obj is Point other && Equals(other);

    // A point is less than another point if its y-value is closer to the origin.
    // If the y-values are the same, then point with the x-value closer to the origin is considered less than the other.
    // This comparison is required to use Point in sets, or sorted vectors.
    public static bool operator < (in Point left, in Point right) => left.CompareTo(right) < 0;
    public static bool operator > (in Point left, in Point right) => left.CompareTo(right) > 0;
    public static bool operator <= (in Point left, in Point right) => left.CompareTo(right) <= 0;
    public static bool operator >= (in Point left, in Point right) => left.CompareTo(right) >= 0;

    public static bool operator == (in Point left, in Point right) => left.Equals(right);

    public static bool operator != (in Point left, in Point right) => !left.Equals(right);

    public void operator +=(in Vector2D vector)
    {
        X = ClampAdd(X, vector.X);
        Y = ClampAdd(Y, vector.Y);
    }

    public void operator -=(in Vector2D vector)
    {
        X = ClampSub(X, vector.X);
        Y = ClampSub(Y, vector.Y);
    }

    public static Point operator +(in Point lhs, in Vector2D rhs)
    {
        Point result = lhs;
        result += rhs;
        return result;
    }

    public static Point operator -(in Point lhs, in Vector2D rhs)
    {
        Point result = lhs;
        result -= rhs;
        return result;
    }

    public static Vector2D operator -(in Point lhs, in Point rhs)
    {
        return new Vector2D(ClampSub(lhs.X, rhs.X), ClampSub(lhs.Y, rhs.Y));
    }
}
