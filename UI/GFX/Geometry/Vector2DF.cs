using System.Runtime.CompilerServices;

namespace UI.GFX.Geometry;

public struct Vector2DF
{
    public float X;
    public float Y;

    public Vector2DF() => (X, Y) = (0f, 0f);

    public Vector2DF(float x, float y) => (X, Y) = (x, y);

    // True if both components of the vector are 0.
    public readonly bool IsZero() => X == 0f && Y == 0f;

    // Add the components of the |other| vector to the current vector.
    public void Add(in Vector2DF other)
    {
        X += other.X;
        Y += other.Y;
    }

    // Subtract the components of the |other| vector from the current vector.
    public void Subtract(in Vector2DF other)
    {
        X -= other.X;
        Y -= other.Y;
    }

    public void SetToMin(in Vector2DF other)
    {
        X = Math.Min(X, other.X);
        Y = Math.Min(Y, other.Y);
    }

    public void SetToMax(in Vector2DF other)
    {
        X = Math.Max(X, other.X);
        Y = Math.Max(Y, other.Y);
    }

    // Gives the square of the diagonal length, i.e. the square of magnitude, of the vector.
    public readonly double LengthSquared() => (double)X * X + (double)Y * Y;

    // Gives the diagonal length (i.e. the magnitude) of the vector.
    public readonly float Length() => float.Hypot(X, Y); // (float)Math.Sqrt(LengthSquared());

    public readonly float AspectRatio() => X / Y;

    // Gives the slope angle in radians of the vector from the positive x axis,
    // in the range of (-pi, pi]. The sign of the result is the same as the sign
    // of y(), except that the result is pi for Vector2dF(negative-x, zero-y).
    public readonly float SlopeAngleRadians()
    {
        return MathF.Atan2(Y, X);
    }

    // Scale the x and y components of the vector by |scale|.
    public void Scale(float scale)
    {
        X *= scale;
        Y *= scale;
    }

    // Scale the x and y components of the vector by |x_scale| and |y_scale|
    // respectively.
    public void Scale(float x_scale, float y_scale)
    {
        X *= x_scale;
        Y *= y_scale;
    }

    // Divides all components of the vector by |scale|.
    public void InvScale(float inv_scale) => InvScale(inv_scale, inv_scale);

    // Divides each component of the vector by the given scale factors.
    public void InvScale(float inv_x_scale, float inv_y_scale)
    {
        X /= inv_x_scale;
        Y /= inv_y_scale;
    }

    public void Normalize() => InvScale(Length());

    public void Transpose()
    {
        (X, Y) = (Y, X);
    }

    // Return the cross product of two vectors, i.e. the determinant.
    public static double CrossProduct(in Vector2DF lhs, in Vector2DF rhs) => (double)lhs.X * rhs.Y - (double)lhs.Y * rhs.X;
    // Return the dot product of two vectors.
    public static double DotProduct(in Vector2DF lhs, in Vector2DF rhs) => (double)lhs.X * rhs.X + (double)lhs.Y * rhs.Y;

    // Return a vector that is |v| scaled by the given scale factors along each axis.
    public static Vector2DF ScaleVector2D(in Vector2DF v, float x_scale, float y_scale)
    {
        return new Vector2DF(v.X * x_scale, v.Y * y_scale);
    }

    // Return a vector that is |v| scaled by the given scale factor.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static  Vector2DF ScaleVector2D(in Vector2DF v, float scale)
    {
        return ScaleVector2D(v, scale, scale);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2DF TransposeVector2D(in Vector2DF v)
    {
        return new Vector2DF(v.Y, v.X);
    }

    // Return a unit vector with the same direction as v.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2DF NormalizeVector2d(in Vector2DF v)
    {
        Vector2DF normal = v;
        normal.Normalize();
        return normal;
    }

    public override readonly string ToString() => $"[{X} {Y}]";

    public override readonly int GetHashCode() => HashCode.Combine(X, Y);

    public readonly bool Equals(in Vector2DF other) => X == other.X && Y == other.Y;

    public override readonly bool Equals(object? obj) => obj is Vector2DF other && Equals(other);

    public static bool operator ==(in Vector2DF left, in Vector2DF right) => left.Equals(right);

    public static bool operator !=(in Vector2DF left, in Vector2DF right) => !left.Equals(right);

    public void operator +=(in Vector2DF other)
    {
        Add(other);
    }

    public void operator -=(in Vector2DF other)
    {
        Subtract(other);
    }

    public static Vector2DF operator -(in Vector2DF v)
    {
        return new Vector2DF(-v.X, -v.Y);
    }

    public static Vector2DF operator +(in Vector2DF lhs, in Vector2DF rhs)
    {
        Vector2DF result = lhs;
        result.Add(rhs);
        return result;
    }

    public static Vector2DF operator -(in Vector2DF lhs, in Vector2DF rhs)
    {
        Vector2DF result = lhs;
        result.Add(-rhs);
        return result;
    }
}
