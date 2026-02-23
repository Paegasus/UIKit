using static UI.Numerics.ClampedMath;
using static UI.GFX.Geometry.InsetsConversions;

namespace UI.GFX.Geometry;

//// <summary>
/// Represents the widths of the four borders or margins of an unspecified
/// rectangle. It stores the thickness of the top, left, bottom and right
/// edges, without storing the actual size and position of the rectangle itself.
///
/// This can be used to represent a space within a rectangle, by "shrinking" the
/// rectangle by the inset amount on all four sides. Alternatively, it can
/// represent a border that has a different thickness on each side.
/// </summary>
public struct Insets
{
    private int m_Top;
    private int m_Left;
    private int m_Bottom;
    private int m_Right;

    public readonly int Top => m_Top;
    public readonly int Left => m_Left;
    public readonly int Bottom => m_Bottom;
    public readonly int Right => m_Right;

    public Insets(int all)
    {
        m_Top = all;
        m_Left = all;
        m_Bottom = ClampBottomOrRight(all, all);
        m_Right = ClampBottomOrRight(all, all);
    }

    public Insets(int vertical, int horizontal)
    {
        m_Top = vertical;
        m_Left = horizontal;
        m_Bottom = ClampBottomOrRight(vertical, vertical);
        m_Right = ClampBottomOrRight(horizontal, horizontal);
    }
    
    public Insets(int top, int left, int bottom, int right)
    {
        m_Top = top;
        m_Left = left;
        m_Bottom = ClampBottomOrRight(top, bottom);
        m_Right = ClampBottomOrRight(left, right);
    }

    /// <summary>
    /// Returns the total width taken up by the insets/outsets, which is the sum of the left and right insets/outsets.
    /// </summary>
    public readonly int Width => m_Left + m_Right;

    /// <summary>
    /// Returns the total height taken up by the insets/outsets, which is the sum of the top and bottom insets/outsets.
    /// </summary>
    public readonly int Height => m_Top + m_Bottom;

    /// <summary>
    /// Returns the sum of the left and right insets/outsets as the width, the sum of the top and bottom insets/outsets as the height.
    /// </summary>
    public readonly Size Size => new(Width, Height);

    /// <summary>
    /// Returns true if the insets/outsets are empty.
    /// </summary>
    public readonly bool IsEmpty => Width == 0 && Height == 0;

    /// <summary>
    /// Flips x- and y-axes.
    /// </summary>
    public void Transpose()
    {
        (m_Top, m_Left) = (m_Left, m_Top);
        (m_Bottom, m_Right) = (m_Right, m_Bottom);
    }

    // These setters can be used together with the default constructor and the
    // single-parameter constructor to construct Insets instances, for example:
    //                                                                  // T, L, B, R
    //   Insets a = Insets().set_top(2);                                // 2, 0, 0, 0
    //   Insets b = Insets().set_left(2).set_bottom(3);                 // 0, 2, 3, 0
    //   Insets c = Insets().set_left_right(1, 2).set_top_bottom(3, 4); // 3, 1, 4, 2
    //   Insets d = Insets(1).set_top(5);                               // 5, 1, 1, 1

    public Insets SetTop(int top)
    {
        m_Top = top;
        m_Bottom = ClampBottomOrRight(m_Top, m_Bottom);
        return this;
    }

    public Insets SetLeft(int left)
    {
        m_Left = left;
        m_Right = ClampBottomOrRight(m_Left, m_Right);
        return this;
    }

    public Insets SetBottom(int bottom)
    {
        m_Bottom = ClampBottomOrRight(m_Top, bottom);
        return this;
    }

    public Insets SetRight(int right)
    {
        m_Right = ClampBottomOrRight(m_Left, right);
        return this;
    }

    // These are preferred to the above setters when setting a pair of edges
    // because these have less clamping and better performance.
    
    public Insets SetLeftRight(int left, int right)
    {
        m_Left = left;
        m_Right = ClampBottomOrRight(m_Left, right);
        return this;
    }

    public Insets SetTopBottom(int top, int bottom)
    {
        m_Top = top;
        m_Bottom = ClampBottomOrRight(m_Top, bottom);
        return this;
    }

    // In addition to the above, we can also use the following methods to
    // construct Insets/Outsets.
    // TLBR() is for Chomium UI code. We should not use it in blink code because
    // the order of parameters is different from the normal orders used in blink.
    // Blink code can use the above setters and VH().
    public static Insets TLBR(int top, int left, int bottom, int right) => new (top, left, bottom, right);

    public static Insets VH(int vertical, int horizontal) => new (vertical, horizontal);
    
    // Sets each side to the maximum of the side and the corresponding side of |other|.
    public void SetToMax(in Insets other)
    {
        m_Top = Math.Max(m_Top, other.m_Top);
        m_Left = Math.Max(m_Left, other.m_Left);
        m_Bottom = Math.Max(m_Bottom, other.m_Bottom);
        m_Right = Math.Max(m_Right, other.m_Right);
    }

    // Conversion from Insets to Outsets negates all components.
    public readonly Outsets ToOutsets() => new (-Top, -Left, -Bottom, -Right);
    
    /// <summary>
    /// Adjusts the vertical and horizontal dimensions by the values described in |vector|.
    /// Offsetting insets before applying to a rectangle would be equivalent to offsetting
    /// the rectangle then applying the insets.
    /// </summary>
    public void Offset(in Vector2D vector)
    {
        SetLeftRight(ClampAdd(Left, vector.X), ClampSub(Right, vector.X));
        SetTopBottom(ClampAdd(Top, vector.Y), ClampSub(Bottom, vector.Y));
    }
    
    public static explicit operator InsetsF(in Insets i) => new InsetsF(i.Top, i.Left, i.Bottom, i.Right);

    public override readonly string ToString() => $"x:{m_Left},{m_Right} y:{m_Top},{m_Bottom}";

    public override readonly int GetHashCode() => HashCode.Combine(m_Top, m_Left, m_Bottom, m_Right);

    public override readonly bool Equals(object? obj) => obj is Insets other && Equals(other);

    public readonly bool Equals(in Insets other) => m_Top == other.m_Top && m_Left == other.m_Left && m_Bottom == other.m_Bottom && m_Right == other.m_Right;
    
    public static bool operator ==(in Insets left, in Insets right) => left.Equals(right);
    public static bool operator !=(in Insets left, in Insets right) => !left.Equals(right);

    public void operator +=(in Insets other)
    {
        m_Top = ClampAdd(m_Top, other.m_Top);
        m_Left = ClampAdd(m_Left, other.m_Left);
        m_Bottom = ClampBottomOrRight(m_Top, ClampAdd(m_Bottom, other.m_Bottom));
        m_Right = ClampBottomOrRight(m_Left, ClampAdd(m_Right, other.m_Right));
    }

    public void operator -=(in Insets other)
    {
        m_Top = ClampSub(m_Top, other.m_Top);
        m_Left = ClampSub(m_Left, other.m_Left);
        m_Bottom = ClampBottomOrRight(m_Top, ClampSub(m_Bottom, other.m_Bottom));
        m_Right = ClampBottomOrRight(m_Left, ClampSub(m_Right, other.m_Right));
    }

    public static Insets operator -(in Insets insets)
    {
        return new Insets(
            SaturatingNegate(insets.Top),
            SaturatingNegate(insets.Left),
            SaturatingNegate(insets.Bottom),
            SaturatingNegate(insets.Right)
        );
    }
    
    public static Insets operator +(Insets a, in Insets b)
    {
        a += b;
        return a;
    }
    
    public static Insets operator -(Insets a, in Insets b)
    {
        a -= b;
        return a;
    }
    
    public static Insets operator +(Insets insets, in Vector2D offset)
    {
        insets.Offset(offset);
        return insets;
    }
    
    // Clamp the bottom/right to avoid integer over/underflow in width() and
    // height(). This returns the clamped bottom/right given a |top_or_left| and
    // a |bottom_or_right|.
    private static int ClampBottomOrRight(int top_or_left, int bottom_or_right) => ClampAdd(top_or_left, bottom_or_right) - top_or_left;

    // Helper methods to scale a Insets to a new Insets.

    public static Insets ScaleToCeiledInsets(in Insets insets, float x_scale, float y_scale)
    {
        if (x_scale == 1.0f && y_scale == 1.0f)
            return insets;

        return ToCeiledInsets(InsetsF.ScaleInsets((InsetsF)insets, x_scale, y_scale));
    }

    public static Insets ScaleToCeiledInsets(in Insets insets, float scale)
    {
        if (scale == 1.0f)
            return insets;
        return ToCeiledInsets(InsetsF.ScaleInsets((InsetsF)insets, scale));
    }

    public static Insets ScaleToFlooredInsets(in Insets insets, float x_scale, float y_scale)
    {
        if (x_scale == 1.0f && y_scale == 1.0f)
            return insets;
        return ToFlooredInsets(InsetsF.ScaleInsets((InsetsF)insets, x_scale, y_scale));
    }

    public static Insets ScaleToFlooredInsets(in Insets insets, float scale)
    {
        if (scale == 1.0f)
            return insets;
        return ToFlooredInsets(InsetsF.ScaleInsets((InsetsF)insets, scale));
    }

    public static Insets ScaleToRoundedInsets(in Insets insets, float x_scale, float y_scale)
    {
        if (x_scale == 1.0f && y_scale == 1.0f)
            return insets;
        return ToRoundedInsets(InsetsF.ScaleInsets((InsetsF)insets, x_scale, y_scale));
    }

    public static Insets ScaleToRoundedInsets(in Insets insets, float scale)
    {
        if (scale == 1.0f)
            return insets;
        return ToRoundedInsets(InsetsF.ScaleInsets((InsetsF)insets, scale));
    }

    private static int SaturatingNegate(int value) => value == int.MinValue ? int.MaxValue : -value;
}
