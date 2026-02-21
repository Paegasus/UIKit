using static UI.Numerics.ClampedMath;

namespace UI.GFX.Geometry;

public struct Vector2D
{
    public int X;
    public int Y;

    public Vector2D() => (X, Y) = (0, 0);

    public Vector2D(int x, int y) => (X, Y) = (x, y);

    // True if both components of the vector are 0.
    public readonly bool IsZero() => X == 0 && Y == 0;

    // Add the components of the |other| vector to the current vector.
    public void Add(in Vector2D other)
    {
        X = ClampAdd(other.X, X);
        Y = ClampAdd(other.Y, Y);
    }

    // Subtract the components of the |other| vector from the current vector.
    public void Subtract(in Vector2D other)
    {
        X = ClampSub(X, other.X);
        Y = ClampSub(Y, other.Y);
    }

    public void SetToMin(in Vector2D other)
    {
        X = Math.Min(X, other.X);
        Y = Math.Min(Y, other.Y);
    }
  
    public void SetToMax(in Vector2D other)
    {
        X = Math.Max(X, other.X);
        Y = Math.Max(Y, other.Y);
    }

    // Gives the square of the diagonal length of the vector. Since this is
    // cheaper to compute than Length(), it is useful when you want to compare
    // relative lengths of different vectors without needing the actual lengths.
    public readonly long LengthSquared() => (long)X * X + (long)Y * Y;

    // Gives the diagonal length of the vector.
    public readonly float Length()
    {
        return (float)Math.Sqrt((double)LengthSquared());
    }

    public void Transpose()
    {
        (X, Y) = (Y, X);
    }

    public override readonly string ToString() => $"[{X} {Y}]";

    public readonly bool Equals(Vector2D other) => X == other.X && Y == other.Y;

    public override readonly bool Equals(object? obj) => obj is Vector2D other && Equals(other);

    public override readonly int GetHashCode() => HashCode.Combine(Y, X);

    public static bool operator == (in Vector2D left, in Vector2D right) => left.Equals(right);
    public static bool operator != (in Vector2D left, in Vector2D right) => !left.Equals(right);

    public void operator +=(in Vector2D other)
    {
        Add(other);
    }

    public void operator -=(in Vector2D other)
    {
        Subtract(other);
    }

    public static Vector2D operator -(in Vector2D v)
    {
        // Negation can overflow for int.MinValue, so we use ClampSub from zero.
        return new Vector2D(ClampSub(0, v.X), ClampSub(0, v.Y));
    }

    public static Vector2D operator +(in Vector2D lhs, in Vector2D rhs)
    {
        Vector2D result = lhs;
        result.Add(rhs);
        return result;
    }

    public static Vector2D operator -(in Vector2D lhs, in Vector2D rhs)
    {
        Vector2D result = lhs;
        result.Subtract(rhs);
        return result;
    }

    public static implicit operator Vector2DF(in Vector2D source)
    {
        return new Vector2DF((float)source.X, (float)source.Y);
    }

    public static Vector2D TransposeVector2D(in Vector2D vector)
    {
        return new Vector2D(vector.Y, vector.X);
    }
}
