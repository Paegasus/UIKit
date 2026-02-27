using System.Runtime.CompilerServices;
using UI.Extensions;

namespace UI.GFX.Geometry;

public struct SizeF
{
    private float m_Width, m_Height;
    public float Width { readonly get => m_Width; set => m_Width = Clamp(value); }
    public float Height { readonly get => m_Height; set => m_Height = Clamp(value); }

    // Threshold for treating very small values as effectively zero.
    // 8 * MachineEpsilon is slightly above float precision noise, and helps avoid
    // tiny rounding artifacts (and denormal-ish values) from propagating into geometry/layout computations.
    public static readonly float Trivial = 8.0f * float.MachineEpsilon;

    // Clamps extremely small values to 0 to suppress floating-point noise.
    // This improves stability of repeated UI math operations (offsetting/scaling),
    // where tiny residuals could otherwise accumulate and cause jitter.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float Clamp(float f) => f > Trivial ? f : 0.0f;

    private static float Next(float f)
    {
        float x = MathF.Max(Trivial, f);
        return MathF.BitIncrement(x);
    }
    
    public SizeF()
    {
        Width = 0f;
        Height = 0f;
    }

    public SizeF(float width, float height)
    {
        Width = width;
        Height = height;
    }

    public SizeF(in Size size)
    {
        Width = size.Width;
        Height = size.Height;
    }

    public readonly float GetArea() => Width * Height;

    public readonly float AspectRatio() => Width / Height;

    public void SetSize(float width, float height)
    {
        Width = width;
        Height = height;
    }

    public void Enlarge(float growWidth, float growHeight)
    {
        SetSize(Width + growWidth, Height + growHeight);
    }

    public void SetToMin(in SizeF other)
    {
        Width = MathF.Min(Width, other.Width);
        Height = MathF.Min(Height, other.Height);
    }

    public void SetToMax(in SizeF other)
    {
        Width = MathF.Max(Width, other.Width);
        Height = MathF.Max(Height, other.Height);
    }
    
    // Expands width/height to the next representable value.
    public void SetToNextWidth() => Width = Next(Width);
    public void SetToNextHeight() => Height = Next(Height);

    public readonly bool IsEmpty => Width == 0.0f || Height == 0.0f;
    public readonly bool IsZero => Width == 0.0f && Height == 0.0f;

    public void Scale(float scale) => Scale(scale, scale);

    public void Scale(float x_scale, float y_scale)
    {
        Width *= x_scale;
        Height *= y_scale;
    }

    // Scales the size by the inverse of the given scale (by dividing).
    public void InvScale(float inv_scale) => InvScale(inv_scale, inv_scale);

    public void InvScale(float inv_x_scale, float inv_y_scale)
    {
        Width /= inv_x_scale;
        Height /= inv_y_scale;
    }

    public void Transpose()
    {
        (Width, Height) = (Height, Width);
    }

    public static SizeF ScaleSize(in SizeF s, float x_scale, float y_scale)
    {
        SizeF scaled_s = s;
        scaled_s.Scale(x_scale, y_scale);
        return scaled_s;
    }

    public static SizeF ScaleSize(in SizeF s, float scale) => ScaleSize(s, scale, scale);

    public static SizeF TransposeSize(in SizeF s) => new(s.Height, s.Width);

    public override readonly string ToString() => $"{Width}x{Height}";

    public override readonly int GetHashCode() => HashCode.Combine(Width, Height);

    public readonly bool Equals(in SizeF other) => Width == other.Width && Height == other.Height;

    public override readonly bool Equals(object? obj) => obj is SizeF other && Equals(other);

    public static bool operator ==(in SizeF left, in SizeF right) => left.Equals(right);
    public static bool operator !=(in SizeF left, in SizeF right) => !left.Equals(right);

    public static SizeF operator +(in SizeF a, in SizeF b)
    {
        return new SizeF(a.Width + b.Width, a.Height + b.Height);
    }

    public static SizeF operator -(in SizeF a, in SizeF b)
    {
        return new SizeF(a.Width - b.Width, a.Height - b.Height);
    }

    public void operator +=(SizeF size)
    {
        SetSize(Width + size.Width, Height + size.Height);
    }

    public void operator -=(SizeF size)
    {
        SetSize(Width - size.Width, Height - size.Height);
    }

    public static explicit operator SizeF(in Size size) => new(size);
}
