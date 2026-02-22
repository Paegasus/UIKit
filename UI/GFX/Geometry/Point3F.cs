namespace UI.GFX.Geometry;

// A point has an x, y and z coordinate.
public struct Point3F : IEquatable<Point3F>
{
    public float X;
    public float Y;
    public float Z;

    public Point3F() => (X, Y, Z) = (0f, 0f, 0f);

    public Point3F(float x, float y, float z) => (X, Y, Z) = (x, y, z);

    public Point3F(in PointF point) => (X, Y, Z) = (point.X, point.Y, 0f);

    public void SetPoint(float x, float y, float z) => (X, Y, Z) = (x, y, z);

    public void Scale(float scale) => Scale(scale, scale, scale);

    public void Scale(float x_scale, float y_scale, float z_scale) => SetPoint(X * x_scale, Y * y_scale, Z * z_scale);

    public readonly bool IsOrigin() => X == 0f && Y == 0f && Z == 0f;

    // Returns the squared euclidean distance between two points.
    public readonly float SquaredDistanceTo(in Point3F other)
    {
        float dx = X - other.X;
        float dy = Y - other.Y;
        float dz = Z - other.Z;
        return dx * dx + dy * dy + dz * dz;
    }

    public readonly PointF AsPointF() => new (X, Y);

    public readonly Vector3DF OffsetFromOrigin() => new (X, Y, Z);

    // Returns a string representation of 3d point.
    public override readonly string ToString() => $"{X},{Y},{Z}";

    public override readonly int GetHashCode() => HashCode.Combine(X, Y, Z);

    public override readonly bool Equals(object? obj) => obj is Point3F other && Equals(other);

    public readonly bool Equals(Point3F other) => X == other.X && Y == other.Y && Z == other.Z;

    public static bool operator ==(in Point3F left, in Point3F right) => left.Equals(right);
    public static bool operator !=(in Point3F left, in Point3F right) => !left.Equals(right);

    // Add a vector to a point, producing a new point offset by the vector.
    public static Point3F operator +(in Point3F a, in Vector3DF b) => new (a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    // Subtract a vector from a point, producing a new point offset by the vector's inverse.
    public static Point3F operator -(in Point3F a, in Vector3DF b) => new (a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    // Subtract one point from another, producing a vector that represents the distances between the two points along each axis.
    public static Vector3DF operator -(in Point3F a, in Point3F b) => new (a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    // Offset the point by the given vector.
    public void operator +=(in Vector3DF v)
    {
        X += v.X;
        Y += v.Y;
        Z += v.Z;
    }

    // Offset the point by the given vector's inverse.
    public void operator -=(in Vector3DF v)
    {
        X -= v.X;
        Y -= v.Y;
        Z -= v.Z;
    }

    public static Point3F PointAtOffsetFromOrigin(in Vector3DF offset) => new Point3F(offset.X, offset.Y, offset.Z);

    public static Point3F ScalePoint(in Point3F p, float x_scale, float y_scale, float z_scale) => new (p.X * x_scale, p.Y * y_scale, p.Z * z_scale);

    public static Point3F ScalePoint(in Point3F p, in Vector3DF v) => new (p.X * v.X, p.Y * v.Y, p.Z * v.Z);

    public static Point3F ScalePoint(in Point3F p, float scale) => ScalePoint(p, scale, scale, scale);
}
