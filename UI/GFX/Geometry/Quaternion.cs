using System.Runtime.CompilerServices;

namespace UI.GFX.Geometry;

// Contains the components of a factored transform.
// These components may be blended and recomposed.
public struct Quaternion
{
    public double X;
    public double Y;
    public double Z;
    public double W;

    const double kEpsilon = 1e-5;

    public Quaternion() => (X, Y, Z, W) = (0.0, 0.0, 0.0, 1.0);

    public Quaternion(double x, double y, double z, double w) => (X, Y, Z, W) = (x, y, z, w);

    public Quaternion(in Vector3DF axis, double angle) : this()
    {
        // Rotation angle is the product of |angle| and the magnitude of |axis|.
        double length = axis.Length();

        if (Math.Abs(length) < kEpsilon)
            return;

        Vector3DF normalized = axis;
        normalized.InvScale((float)length);

        angle *= 0.5;
        double s = Math.Sin(angle);
        X = normalized.X * s;
        Y = normalized.Y * s;
        Z = normalized.Z * s;
        W = Math.Cos(angle);
    }

    // Constructs a quaternion representing a rotation between |from| and |to|.
    public Quaternion(in Vector3DF from, in Vector3DF to) : this()
    {
        double dot = Vector3DF.DotProduct(from, to);
        double norm = Math.Sqrt(from.LengthSquared() * to.LengthSquared());
        double real = norm + dot;
        Vector3DF axis;
        if (real < kEpsilon * norm)
        {
            real = 0.0f;
            axis =  MathF.Abs(from.X) > MathF.Abs(from.Z) ?
                    new Vector3DF( -from.Y, from.X, 0f) :
                    new Vector3DF(0f, -from.Z, from.Y);
        }
        else
        {
            axis = Vector3DF.CrossProduct(from, to);
        }
        X = axis.X;
        Y = axis.Y;
        Z = axis.Z;
        W = real;
        this = this.Normalized();
    }

    public static Quaternion FromAxisAngle(double x, double y, double z, double angle)
    {
        double length = Math.Sqrt(x * x + y * y + z * z);

        if (Math.Abs(length) < kEpsilon)
            return new Quaternion(0, 0, 0, 1);

        double scale = Math.Sin(0.5 * angle) / length;

        return new Quaternion(scale * x, scale * y, scale * z, Math.Cos(0.5 * angle));
    }

    public readonly Quaternion inverse()
    {
        return new Quaternion(-X, -Y, -Z, W);
    }

    public readonly Quaternion flip()
    {
        return new Quaternion(-X, -Y, -Z, -W);
    }

    // Adapted from https://www.euclideanspace.com/maths/algebra/realNormedAlgebra/
    // quaternions/slerp/index.htm
    // Blends with the given quaternion, |q|, via spherical linear interpolation.
    // Values of |t| in the range [0, 1] will interpolate between |this| and |q|,
    // and values outside that range will extrapolate beyond in either direction.
    public readonly Quaternion Slerp(in Quaternion to, double t)
    {
        Quaternion from = this;

        double cos_half_angle = from.X * to.X + from.Y * to.Y + from.Z * to.Z + from.W * to.W;

        if (cos_half_angle < 0)
        {
            // Since the half angle is > 90 degrees, the full rotation angle would
            // exceed 180 degrees. The quaternions (x, y, z, w) and (-x, -y, -z, -w)
            // represent the same rotation. Flipping the orientation of either
            // quaternion ensures that the half angle is less than 90 and that we are
            // taking the shortest path.
            from = from.flip();
            cos_half_angle = -cos_half_angle;
        }

        // Ensure that acos is well behaved at the boundary.
        if (cos_half_angle > 1)
            cos_half_angle = 1;

        double sin_half_angle = Math.Sqrt(1.0 - cos_half_angle * cos_half_angle);
        if (sin_half_angle < kEpsilon)
        {
            // Quaternions share common axis and angle.
            return this;
        }

        double half_angle = Math.Acos(cos_half_angle);

        double scaleA = Math.Sin((1 - t) * half_angle) / sin_half_angle;
        double scaleB = Math.Sin(t * half_angle) / sin_half_angle;

        return (scaleA * from) + (scaleB * to);
    }

    // Blends with the given quaternion, |q|, via linear interpolation. This is
    // rarely what you want. Use only if you know what you're doing.
    // Values of |t| in the range [0, 1] will interpolate between |this| and |q|,
    // and values outside that range will extrapolate beyond in either direction.
    public readonly Quaternion Lerp(in Quaternion q, double t)
    {
        var test = 1.0 * this;
        
        return (((1.0 - t) * this) + (t * q)).Normalized();
    }

    public readonly double Length()
    {
        return X * X + Y * Y + Z * Z + W * W;
    }

    public readonly Quaternion Normalized()
    {
        double length = Length();

        if (length < kEpsilon)
            return this;

        return this / Math.Sqrt(length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion operator +(in Quaternion a, in Quaternion b)
    {
        return new Quaternion(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion operator *(in Quaternion a, in Quaternion b)
    {
        return new Quaternion(
            a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
            a.W * b.Y - a.X * b.Z + a.Y * b.W + a.Z * b.X,
            a.W * b.Z + a.X * b.Y - a.Y * b.X + a.Z * b.W,
            a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z
        );
    }

    // |s| is an arbitrary, real constant.
    public static Quaternion operator *(in Quaternion q, double scalar)
    {
        return new Quaternion(q.X * scalar, q.Y * scalar, q.Z * scalar, q.W * scalar);
    }

    // |s| is an arbitrary, real constant.
    public static Quaternion operator *(double scalar, in Quaternion quaternion)
    {
        return new Quaternion(quaternion.X * scalar,
                              quaternion.Y * scalar, 
                              quaternion.Z * scalar, 
                              quaternion.W * scalar);
    }

    // |s| is an arbitrary, real constant.
    public static Quaternion operator /(in Quaternion quaternion, double scalar)
    {
        double inv = 1.0 / scalar;
        return quaternion * inv;
    }

    public override readonly string ToString()
    {
        // q = (con(abs(v_theta)/2), v_theta/abs(v_theta) * sin(abs(v_theta)/2))
        float abs_theta = (float)Math.Acos(W) * 2;
        float scale = 1.0f / MathF.Sin(abs_theta * 0.5f);
        Vector3DF v = new((float)X, (float)Y, (float)Z);
        v.Scale(scale);
        return $"[{X} {Y} {Z} {W}], v:{v}, θ:{abs_theta / MathF.PI}π";
    }

    public override readonly int GetHashCode() => HashCode.Combine(X, Y, Z, W);
    
    public readonly bool Equals(in Quaternion other) => X == other.X && Y == other.Y && Z == other.Z && W == other.W;

    public override readonly bool Equals(object? obj) => obj is Quaternion other && Equals(other);

    public static bool operator == (in Quaternion left, in Quaternion right) => left.Equals(right);
    public static bool operator != (in Quaternion left, in Quaternion right) => !left.Equals(right);
}
