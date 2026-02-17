using System.Runtime.CompilerServices;

using static UI.Numerics.AngleConversions;

namespace UI.GFX.Geometry;

/// <summary>
/// A 3D vector of floats, used to represent a distance in 3D space.
/// </summary>
public struct Vector3DF : IEquatable<Vector3DF>
{
    private const double Epsilon = 1.0e-6;

    public float X;
    public float Y;
    public float Z;

    public Vector3DF() => (X, Y, Z) = (0f, 0f, 0f);

    public Vector3DF(float x, float y, float z) => (X, Y, Z) = (x, y, z);
    
    public Vector3DF(in Vector2DF vector) => (X, Y, Z) = (vector.X, vector.Y, 0f);

    /// <summary>
    /// Checks if all components of the vector are zero.
    /// </summary>
    public readonly bool IsZero() => X == 0f && Y == 0f && Z == 0f;

    /// <summary>
    /// Adds the components of the other vector to this vector.
    /// </summary>
    public void Add(in Vector3DF other)
    {
        X += other.X;
        Y += other.Y;
        Z += other.Z;
    }

    /// <summary>
    /// Subtracts the components of the other vector from this vector.
    /// </summary>
    public void Subtract(in Vector3DF other)
    {
        X -= other.X;
        Y -= other.Y;
        Z -= other.Z;
    }

    public void SetToMin(in Vector3DF other)
    {
        X = Math.Min(X, other.X);
        Y = Math.Min(Y, other.Y);
        Z = Math.Min(Z, other.Z);
    }

    public void SetToMax(in Vector3DF other)
    {
        X = Math.Max(X, other.X);
        Y = Math.Max(Y, other.Y);
        Z = Math.Max(Z, other.Z);
    }

    /// <summary>
    /// Returns the square of the vector's length.
    /// </summary>
    public readonly double LengthSquared() => (double)X * X + (double)Y * Y + (double)Z * Z;
    
    /// <summary>
    /// Returns the vector's length.
    /// </summary>
    public readonly float Length() => (float)Math.Sqrt(LengthSquared());
    
    /// <summary>
    /// Scales all components of the vector uniformly by a single scale factor.
    /// </summary>
    public void Scale(float scale) => Scale(scale, scale, scale);

    /// <summary>
    /// Scales each component of the vector by the given scale factors.
    /// </summary>
    public void Scale(float x_scale, float y_scale, float z_scale)
    {
        X *= x_scale;
        Y *= y_scale;
        Z *= z_scale;
    }
    
    /// <summary>
    /// Divides all components of the vector by a single scale factor.
    /// </summary>
    public void InvScale(float inv_scale) => InvScale(inv_scale, inv_scale, inv_scale);

    /// <summary>
    /// Divides each component of the vector by the given scale factors.
    /// </summary>
    public void InvScale(float inv_x_scale, float inv_y_scale, float inv_z_scale)
    {
        X /= inv_x_scale;
        Y /= inv_y_scale;
        Z /= inv_z_scale;
    }
    
    /// <summary>
    /// Computes the cross product of this vector with another and updates this vector with the result.
    /// </summary>
    public void Cross(in Vector3DF other)
    {
        double dx = X;
        double dy = Y;
        double dz = Z;
        float new_x = (float)(dy * other.Z - dz * other.Y);
        float new_y = (float)(dz * other.X - dx * other.Z);
        float new_z = (float)(dx * other.Y - dy * other.X);
        X = new_x;
        Y = new_y;
        Z = new_z;
    }

    /// <summary>
    /// Attempts to compute a unit-length vector in the same direction.
    /// </summary>
    /// <param name="result">The resulting normalized vector.</param>
    /// <returns>True if the vector was successfully normalized, false if the vector is too short (close to zero).</returns>
    public readonly bool GetNormalized(out Vector3DF result)
    {
        result = this;
        double lengthSquared = LengthSquared();
        if (lengthSquared < Epsilon * Epsilon)
            return false;
        result.InvScale((float)Math.Sqrt(lengthSquared));
        return true;
    }
    
    public override readonly string ToString() => $"[{X} {Y} {Z}]";

    public override readonly int GetHashCode() => HashCode.Combine(X, Y, Z);

    public override readonly bool Equals(object? obj) => obj is Vector3DF other && Equals(other);

    public readonly bool Equals(Vector3DF other) => X == other.X && Y == other.Y && Z == other.Z;

    public static bool operator ==(in Vector3DF left, in Vector3DF right) => left.Equals(right);
    public static bool operator !=(in Vector3DF left, in Vector3DF right) => !left.Equals(right);

    public void operator +=(in Vector3DF other)
    {
        Add(other);
    }

    public void operator -=(in Vector3DF other)
    {
        Subtract(other);
    }

    public static Vector3DF operator -(in Vector3DF v)
    {
        return new Vector3DF(-v.X, -v.Y, -v.Z);
    }

    public static Vector3DF operator +(in Vector3DF lhs, in Vector3DF rhs)
    {
        Vector3DF result = lhs;
        result.Add(rhs);
        return result;
    }

    public static Vector3DF operator -(in Vector3DF lhs, in Vector3DF rhs)
    {
        Vector3DF result = lhs;
        result.Add(-rhs);
        return result;
    }

    /// <summary>
    /// Returns the cross product of two vectors.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3DF CrossProduct(in Vector3DF lhs, in Vector3DF rhs)
    {
        Vector3DF result = lhs;
        result.Cross(rhs);
        return result;
    }

    /// <summary>
    /// Returns the dot product of two vectors.
    /// </summary>
    public static float DotProduct(in Vector3DF lhs, in Vector3DF rhs)
    {
        return lhs.X * rhs.X + lhs.Y * rhs.Y + lhs.Z * rhs.Z;
    }

    /// <summary>
    /// Returns a new vector created by scaling the components of |v| by the components of |s|.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3DF ScaleVector(in Vector3DF v, in Vector3DF s)
    {
        return new Vector3DF(v.X * s.X, v.Y * s.Y, v.Z * s.Z);
    }
    
    /// <summary>
    /// Returns a new vector created by scaling |v| by the given scale factors.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3DF ScaleVector(in Vector3DF v, float x_scale, float y_scale, float z_scale)
    {
        return new Vector3DF(v.X * x_scale, v.Y * y_scale, v.Z * z_scale);
    }

    /// <summary>
    /// Returns a new vector created by scaling |v| by a uniform scale factor.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3DF ScaleVector(in Vector3DF v, float scale)
    {
        return ScaleVector(v, scale, scale, scale);
    }
    
    /// <summary>
    /// Returns the angle between two vectors in degrees.
    /// </summary>
    public static float AngleBetweenVectorsInDegrees(in Vector3DF baseVec, in Vector3DF other)
    {
        float angle = RadToDeg(
            MathF.Acos(
                Math.Clamp(DotProduct(baseVec, other) / baseVec.Length() / other.Length(), -1.0f, 1.0f)
            )
        );
        return angle;
    }

    /// <summary>
    /// Returns the clockwise angle between two vectors, viewed from the direction of a normal vector.
    /// </summary>
    public static float ClockwiseAngleBetweenVectorsInDegrees(in Vector3DF baseVec, in Vector3DF other, in Vector3DF normal)
    {
        float angle = AngleBetweenVectorsInDegrees(baseVec, other);
        Vector3DF cross = CrossProduct(baseVec, other);

        // If the dot product of this cross product is normal, it means that the
        // shortest angle between |base| and |other| was counterclockwise and
        // this angle must be reversed.
        if (DotProduct(cross, normal) > 0.0f)
            angle = 360.0f - angle;
        return angle;
    }
}
