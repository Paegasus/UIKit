namespace UI.GFX.Geometry;

using System.Runtime.CompilerServices;
using static Numerics.ClampedMath;

// Contains the components of a factored transform.
// These components may be blended and recomposed.
public struct Quaternion
{
    private double x_ = 0.0;
    private double y_ = 0.0;
    private double z_ = 0.0;
    private double w_ = 1.0;

    public double x { readonly get => x_; set => x_ = value; }
    public double y { readonly get => y_; set => y_ = value; }
    public double z { readonly get => z_; set => z_ = value; }
    public double w { readonly get => w_; set => w_ = value; }

    const double kEpsilon = 1e-5;

    public Quaternion(double x, double y, double z, double w)
    {
        x_ = x;
        y_ = y;
        z_ = z;
        w_ = w;
    }

    public Quaternion(in Vector3DF axis, double angle)
    {
        // Rotation angle is the product of |angle| and the magnitude of |axis|.
        double length = axis.Length();
        if (Math.Abs(length) < kEpsilon)
            return;

        Vector3DF normalized = axis;
        normalized.InvScale((float)length);

        angle *= 0.5;
        double s = Math.Sin(angle);
        x_ = normalized.x * s;
        y_ = normalized.y * s;
        z_ = normalized.z * s;
        w_ = Math.Cos(angle);
    }

    // Constructs a quaternion representing a rotation between |from| and |to|.
    public Quaternion(in Vector3DF from, in Vector3DF to)
    {
        double dot = Vector3DF.DotProduct(from, to);
        double norm = Math.Sqrt(from.LengthSquared() * to.LengthSquared());
        double real = norm + dot;
        Vector3DF axis;
        if (real < kEpsilon * norm)
        {
            real = 0.0f;
            axis =  MathF.Abs(from.x) > MathF.Abs(from.z) ?
                    new Vector3DF( -from.y, from.x, 0f) :
                    new Vector3DF(0f, -from.z, from.y);
        }
        else
        {
            axis = Vector3DF.CrossProduct(from, to);
        }
        x_ = axis.x;
        y_ = axis.y;
        z_ = axis.z;
        w_ = real;
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
        return new Quaternion(-x_, -y_, -z_, w_);
    }

    public readonly Quaternion flip()
    {
        return new Quaternion(-x_, -y_, -z_, -w_);
    }

    // Adapted from https://www.euclideanspace.com/maths/algebra/realNormedAlgebra/
    // quaternions/slerp/index.htm
    // Blends with the given quaternion, |q|, via spherical linear interpolation.
    // Values of |t| in the range [0, 1] will interpolate between |this| and |q|,
    // and values outside that range will extrapolate beyond in either direction.
    public readonly Quaternion Slerp(in Quaternion to, double t)
    {
        Quaternion from = this;

        double cos_half_angle = from.x_ * to.x_ + from.y_ * to.y_ + from.z_ * to.z_ + from.w_ * to.w_;

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
        return x_ * x_ + y_ * y_ + z_ * z_ + w_ * w_;
    }

    public readonly Quaternion Normalized()
    {
        double length = Length();

        if (length < kEpsilon)
            return this;

        return this / Math.Sqrt(length);
    }

    public static Quaternion operator +(in Quaternion a, in Quaternion b)
    {
        return new Quaternion(a.x_ + b.x_, a.y_ + b.y_, a.z_ + b.z_, a.w_ + b.w_);
    }

    public static Quaternion operator *(in Quaternion a, in Quaternion b)
    {
        return new Quaternion(
            a.w_ * b.x_ + a.x_ * b.w_ + a.y_ * b.z_ - a.z_ * b.y_,
            a.w_ * b.y_ - a.x_ * b.z_ + a.y_ * b.w_ + a.z_ * b.x_,
            a.w_ * b.z_ + a.x_ * b.y_ - a.y_ * b.x_ + a.z_ * b.w_,
            a.w_ * b.w_ - a.x_ * b.x_ - a.y_ * b.y_ - a.z_ * b.z_
        );
    }

    // |s| is an arbitrary, real constant.
    public static Quaternion operator *(in Quaternion q, double s)
    {
        return new Quaternion(q.x * s, q.y * s, q.z * s, q.w * s);
    }

    // |s| is an arbitrary, real constant.
    public static Quaternion operator *(double s, in Quaternion q)
    {
        return new Quaternion(q.x * s, q.y * s, q.z * s, q.w * s);
    }

    // |s| is an arbitrary, real constant.
    public static Quaternion operator /(in Quaternion q, double s)
    {
        double inv = 1.0 / s;
        return q * inv;
    }

    public override readonly string ToString()
    {
        // q = (con(abs(v_theta)/2), v_theta/abs(v_theta) * sin(abs(v_theta)/2))
        float abs_theta = (float)Math.Acos(w_) * 2;
        float scale = 1.0f / MathF.Sin(abs_theta * 0.5f);
        Vector3DF v = new((float)x_, (float)y_, (float)z_);
        v.Scale(scale);
        return $"[{x_} {y_} {z_} {w_}], v:{v}, θ:{abs_theta / MathF.PI}π";
    }
}
