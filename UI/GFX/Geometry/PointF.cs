namespace UI.GFX.Geometry;

public struct PointF : IComparable<PointF>
{
    public float X;
    public float Y;

    public PointF() => (X, Y) = (0f, 0f);
    
    public PointF(float x, float y) => (X, Y) = (x, y);

    public PointF(in Point p) => (X, Y) = (p.X, p.Y);

    public void SetPoint(float x, float y) => (X, Y) = (x, y);

    public void Offset(float delta_x, float delta_y)
    {
        X += delta_x;
        Y += delta_y;
    }

    public void SetToMin(in PointF other)
    {
        X = Math.Min(X, other.X);
        Y = Math.Min(Y, other.Y);
    }

    public void SetToMax(in PointF other)
    {
        X = Math.Max(X, other.X);
        Y = Math.Max(Y, other.Y);
    }

    public readonly bool IsOrigin() => X == 0 && Y == 0;
    
    public readonly Vector2DF OffsetFromOrigin() => new (X, Y);

    public void Scale(float scale) => Scale(scale, scale);

    public void Scale(float x_scale, float y_scale) => SetPoint(X * x_scale, Y * y_scale);

    // Scales each component by the inverse of the given scales.
    public void InvScale(float inv_x_scale, float inv_y_scale)
    {
        X /= inv_x_scale;
        Y /= inv_y_scale;
    }

    // Scales the point by the inverse of the given scale.
    public void InvScale(float inv_scale) => InvScale(inv_scale, inv_scale);
    
    public void Transpose() => (X, Y) = (Y, X);

    // Uses the Pythagorean theorem to determine the straight line distance
    // between the two points, and returns true if it is less than
    // |allowed_distance|.
    public readonly bool IsWithinDistance(in PointF rhs, float allowed_distance)
    {
#if DEBUG
        //DCHECK(allowed_distance > 0);
#endif
        float diff_x = X - rhs.X;
        float diff_y = Y - rhs.Y;
        float distance = MathF.Sqrt(diff_x * diff_x + diff_y * diff_y);

        return distance < allowed_distance;
    }

    public static PointF PointAtOffsetFromOrigin(in Vector2DF offset_from_origin) => new (offset_from_origin.X, offset_from_origin.Y);

    public static PointF ScalePoint(in PointF p, float x_scale, float y_scale)
    {
        PointF scaled_p = p;

        scaled_p.Scale(x_scale, y_scale);

        return scaled_p;
    }

    public static PointF ScalePoint(in PointF p, float scale) => ScalePoint(p, scale, scale);

    public static PointF TransposePoint(in PointF p) => new (p.Y, p.X);

    public override readonly string ToString() => $"{X},{Y}";

    public override readonly int GetHashCode() => HashCode.Combine(Y, X);

    public readonly int CompareTo(PointF other)
    {
        int yComparison = Y.CompareTo(other.Y);
        return yComparison != 0 ? yComparison : X.CompareTo(other.X);
    }

    public override readonly bool Equals(object? obj) => obj is PointF other && Equals(other);
    
    public readonly bool Equals(in PointF other) => X == other.X && Y == other.Y;

    // A point is less than another point if its y-value is closer to the origin.
    // If the y-values are the same, then point with the x-value closer to the origin is considered less than the other.
    // This comparison is required to use PointF in sets, or sorted vectors.
    public static bool operator < (in PointF left, in PointF right) => left.CompareTo(right) < 0;
    public static bool operator > (in PointF left, in PointF right) => left.CompareTo(right) > 0;
    public static bool operator <= (in PointF left, in PointF right) => left.CompareTo(right) <= 0;
    public static bool operator >= (in PointF left, in PointF right) => left.CompareTo(right) >= 0;

    public static bool operator == (in PointF left, in PointF right) => left.Equals(right);
    public static bool operator != (in PointF left, in PointF right) => !left.Equals(right);

    public void operator +=(in Vector2DF vector)
    {
        X += vector.X;
        Y += vector.Y;
    }

    public void operator -=(in Vector2DF vector)
    {
        X -= vector.X;
        Y -= vector.Y;
    }

    public static PointF operator +(in PointF lhs, in Vector2DF rhs)
    {
        PointF result = lhs;
        result += rhs;
        return result;
    }

    public static PointF operator -(in PointF lhs, in Vector2DF rhs)
    {
        PointF result = lhs;
        result -= rhs;
        return result;
    }

    public static Vector2DF operator -(in PointF a, in PointF b) => new (a.X - b.X, a.Y - b.Y);
}
