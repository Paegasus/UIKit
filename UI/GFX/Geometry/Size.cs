using static UI.Numerics.ClampedMath;
using static UI.GFX.Geometry.SizeConversions;

namespace UI.GFX.Geometry;

public struct Size
{
    private int m_Width;
    private int m_Height;

    public int Width { readonly get => m_Width; set => m_Width = Math.Max(0, value); }

    public int Height { readonly get => m_Height; set => m_Height = Math.Max(0, value); }

    public Size() => (m_Width, m_Height) = (0, 0);

    public Size(int width, int height) => (Width, Height) = (width, height);

    public void SetSize(int width, int height) => (Width, Height) = (width, height);

    public readonly bool IsEmpty() => m_Width == 0 || m_Height == 0;

    public readonly bool IsZero() => m_Width == 0 && m_Height == 0;

    public void Enlarge(int growWidth, int growHeight)
    {
        Width = ClampAdd(m_Width, growWidth);
        Height = ClampAdd(m_Height, growHeight);
    }

    public void SetToMin(in Size other)
    {
        m_Width = Math.Min(m_Width, other.m_Width);
        m_Height = Math.Min(m_Height, other.m_Height);
    }

    public void SetToMax(in Size other)
    {
        m_Width = Math.Max(m_Width, other.m_Width);
        m_Height = Math.Max(m_Height, other.m_Height);
    }

    public void Transpose()
    {
        (m_Width, m_Height) = (m_Height, m_Width);
    }

    /// <summary>
    /// Returns the area. This method will throw an OverflowException
    /// if the area exceeds the bounds of a 32-bit integer.
    /// </summary>
    public readonly int GetArea() => checked(Width * Height);

    /// <summary>
    /// Returns the area as a 64-bit integer, avoiding overflow issues.
    /// </summary>
    public readonly long Area64() => (long)Width * Height;

    public static Size TransposeSize(in Size s) => new (s.Height, s.Width);

    public static Size ScaleToCeiledSize(in Size size, float x_scale, float y_scale)
    {
        if (x_scale == 1.0f && y_scale == 1.0f)
            return size;

        return ToCeiledSize(SizeF.ScaleSize((SizeF)size, x_scale, y_scale));
    }
    
    public static Size ScaleToCeiledSize(in Size size, float scale)
    {
        if (scale == 1.0f)
            return size;

        return ToCeiledSize(SizeF.ScaleSize((SizeF)size, scale, scale));
    }

    public static Size ScaleToFlooredSize(in Size size, float x_scale, float y_scale)
    {
        if (x_scale == 1.0f && y_scale == 1.0f)
            return size;

        return ToFlooredSize(SizeF.ScaleSize((SizeF)size, x_scale, y_scale));
    }

    public static Size ScaleToFlooredSize(in Size size, float scale)
    {
        if (scale == 1.0f)
            return size;

        return ToFlooredSize(SizeF.ScaleSize((SizeF)size, scale, scale));
    }

    public static Size ScaleToRoundedSize(in Size size, float x_scale, float y_scale)
    {
        if (x_scale == 1.0f && y_scale == 1.0f)
            return size;

        return ToRoundedSize(SizeF.ScaleSize((SizeF)size, x_scale, y_scale));
    }

    public static Size ScaleToRoundedSize(in Size size, float scale)
    {
        if (scale == 1.0f)
            return size;

        return ToRoundedSize(SizeF.ScaleSize((SizeF)size, scale, scale));
    }

    public override readonly string ToString() => $"{m_Width}x{m_Height}";

    public override readonly int GetHashCode() => HashCode.Combine(m_Width, m_Height);

    public readonly bool Equals(in Size other) => Width == other.Width && Height == other.Height;

    public override readonly bool Equals(object? obj) => obj is Size other && Equals(other);

    public static bool operator ==(in Size left, in Size right) => left.Equals(right);
    public static bool operator !=(in Size left, in Size right) => !left.Equals(right);

    public void operator +=(in Size size)
    {
        Enlarge(size.Width, size.Height);
    }

    public void operator -=(in Size size)
    {
        Enlarge(-size.Width, -size.Height);
    }

    public static Size operator +(Size a, in Size b)
    {
        a += b;
        return a;
    }

    public static Size operator -(Size a, in Size b)
    {
        a -= b;
        return a;
    }
}
