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
public struct Insets : IEquatable<Insets>
{
    private int top_;
    private int left_;
    private int bottom_;
    private int right_;

    public int top() => top_;
    public int left() => left_;
    public int bottom() => bottom_;
    public int right() => right_;

    public Insets(int all)
    {
        top_ = all;
        left_ = all;
        bottom_ = ClampBottomOrRight(all, all);
        right_ = ClampBottomOrRight(all, all);
    }

    public Insets(int vertical, int horizontal)
    {
        top_ = vertical;
        left_ = horizontal;
        bottom_ = ClampBottomOrRight(vertical, vertical);
        right_ = ClampBottomOrRight(horizontal, horizontal);
    }
    
    public Insets(int top, int left, int bottom, int right)
    {
        top_ = top;
        left_ = left;
        bottom_ = ClampBottomOrRight(top, bottom);
        right_ = ClampBottomOrRight(left, right);
    }

    /// <summary>
    /// Returns the total width taken up by the insets/outsets, which is the sum of the left and right insets/outsets.
    /// </summary>
    public readonly int width() => left_ + right_;

    /// <summary>
    /// Returns the total height taken up by the insets/outsets, which is the sum of the top and bottom insets/outsets.
    /// </summary>
    public readonly int height() => top_ + bottom_;

    /// <summary>
    /// Returns the sum of the left and right insets/outsets as the width, the sum of the top and bottom insets/outsets as the height.
    /// </summary>
    public readonly Size size() => new Size(width(), height());

    /// <summary>
    /// Returns true if the insets/outsets are empty.
    /// </summary>
    public readonly bool IsEmpty() => width() == 0 && height() == 0;
    
    /// <summary>
    /// Flips x- and y-axes.
    /// </summary>
    public void Transpose()
    {
        (top_, left_) = (left_, top_);
        (bottom_, right_) = (right_, bottom_);
    }

    // These setters can be used together with the default constructor and the
    // single-parameter constructor to construct Insets instances, for example:
    //                                                                  // T, L, B, R
    //   Insets a = Insets().set_top(2);                                // 2, 0, 0, 0
    //   Insets b = Insets().set_left(2).set_bottom(3);                 // 0, 2, 3, 0
    //   Insets c = Insets().set_left_right(1, 2).set_top_bottom(3, 4); // 3, 1, 4, 2
    //   Insets d = Insets(1).set_top(5);                               // 5, 1, 1, 1

    public Insets set_top(int top)
    {
        top_ = top;
        bottom_ = ClampBottomOrRight(top_, bottom_);
        return this;
    }

    public Insets set_left(int left)
    {
        left_ = left;
        right_ = ClampBottomOrRight(left_, right_);
        return this;
    }

    public Insets set_bottom(int bottom)
    {
        bottom_ = ClampBottomOrRight(top_, bottom);
        return this;
    }

    public Insets set_right(int right)
    {
        right_ = ClampBottomOrRight(left_, right);
        return this;
    }

    // These are preferred to the above setters when setting a pair of edges
    // because these have less clamping and better performance.
    
    public Insets set_left_right(int left, int right) {
        left_ = left;
        right_ = ClampBottomOrRight(left_, right);
        return this;
    }

    public Insets set_top_bottom(int top, int bottom) {
        top_ = top;
        bottom_ = ClampBottomOrRight(top_, bottom);
        return this;
    }

    // In addition to the above, we can also use the following methods to
    // construct Insets/Outsets.
    // TLBR() is for Chomium UI code. We should not use it in blink code because
    // the order of parameters is different from the normal orders used in blink.
    // Blink code can use the above setters and VH().
    public static Insets TLBR(int top, int left, int bottom, int right) => new Insets(top, left, bottom, right);

    public static Insets VH(int vertical, int horizontal) => new Insets(vertical, horizontal);
    
    // Sets each side to the maximum of the side and the corresponding side of |other|.
    public void SetToMax(in Insets other)
    {
        top_ = Math.Max(top_, other.top_);
        left_ = Math.Max(left_, other.left_);
        bottom_ = Math.Max(bottom_, other.bottom_);
        right_ = Math.Max(right_, other.right_);
    }

    // Conversion from Insets to Outsets negates all components.
    public Outsets ToOutsets() => new Outsets(-top(), -left(), -bottom(), -right());
    
    /// <summary>
    /// Adjusts the vertical and horizontal dimensions by the values described in |vector|.
    /// Offsetting insets before applying to a rectangle would be equivalent to offsetting
    /// the rectangle then applying the insets.
    /// </summary>
    public void Offset(in Vector2D vector)
    {
        set_left_right(ClampAdd(left(), vector.X), ClampSub(right(), vector.X));
        set_top_bottom(ClampAdd(top(), vector.Y), ClampSub(bottom(), vector.Y));
    }
    
    public static explicit operator InsetsF(in Insets i) => new InsetsF(i.top(), i.left(), i.bottom(), i.right());

    public override readonly string ToString() => $"x:{left_},{right_} y:{top_},{bottom_}";

    public override readonly int GetHashCode() => HashCode.Combine(top_, left_, bottom_, right_);

    public override readonly bool Equals(object? obj) => obj is Insets other && Equals(other);

    public readonly bool Equals(Insets other) =>
        top_ == other.top_ && left_ == other.left_ && bottom_ == other.bottom_ && right_ == other.right_;
    
    public static bool operator ==(in Insets left, in Insets right) => left.Equals(right);
    public static bool operator !=(in Insets left, in Insets right) => !left.Equals(right);

    public void operator +=(in Insets other)
    {
        top_ = ClampAdd(top_, other.top_);
        left_ = ClampAdd(left_, other.left_);
        bottom_ = ClampBottomOrRight(top_, ClampAdd(bottom_, other.bottom_));
        right_ = ClampBottomOrRight(left_, ClampAdd(right_, other.right_));
    }

    public void operator -=(in Insets other)
    {
        top_ = ClampSub(top_, other.top_);
        left_ = ClampSub(left_, other.left_);
        bottom_ = ClampBottomOrRight(top_, ClampSub(bottom_, other.bottom_));
        right_ = ClampBottomOrRight(left_, ClampSub(right_, other.right_));
    }

    public static Insets operator -(in Insets v)
    {
        return new Insets(
            SaturatingNegate(v.top()),
            SaturatingNegate(v.left()),
            SaturatingNegate(v.bottom()),
            SaturatingNegate(v.right())
        );
    }
    
    public static Insets operator +(Insets lhs, in Insets rhs)
    {
        lhs += rhs;
        return lhs;
    }
    
    public static Insets operator -(Insets lhs, in Insets rhs)
    {
        lhs -= rhs;
        return lhs;
    }
    
    public static Insets operator +(Insets insets, in Vector2D offset)
    {
        insets.Offset(offset);
        return insets;
    }
    
    // Clamp the bottom/right to avoid integer over/underflow in width() and
    // height(). This returns the clamped bottom/right given a |top_or_left| and
    // a |bottom_or_right|.
    private static int ClampBottomOrRight(int top_or_left, int bottom_or_right)
    {
        return ClampAdd(top_or_left, bottom_or_right) - top_or_left;
    }

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

    private static int SaturatingNegate(int v) => v == int.MinValue ? int.MaxValue : -v;
}
