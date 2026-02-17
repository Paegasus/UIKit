namespace UI.GFX.Geometry;

//// <summary>
/// This is the floating point version of Insets.
/// </summary>
public struct InsetsF : IEquatable<InsetsF>
{
    private float top_;
    private float left_;
    private float bottom_;
    private float right_;

    public float top() => top_;
    public float left() => left_;
    public float bottom() => bottom_;
    public float right() => right_;

    public InsetsF(float all) 
    {
        top_ = all;
        left_ = all;
        bottom_ = all;
        right_ = all;
    }

    public InsetsF(float vertical, float horizontal)
    {
        top_ = vertical;
        left_ = horizontal;
        bottom_ = vertical;
        right_ = horizontal;
    }

    public InsetsF(float top, float left, float bottom, float right)
    {
        top_ = top;
        left_ = left;
        bottom_ = bottom;
        right_ = right;
    }

    // Returns the total width taken up by the insets/outsets, which is the
    // sum of the left and right insets/outsets.
    public readonly float width() => left_ + right_;

    // Returns the total height taken up by the insets/outsets, which is the
    // sum of the top and bottom insets/outsets.
    public readonly float height() => top_ + bottom_;

    // Returns true if the insets/outsets are empty.
    public readonly bool IsEmpty() => width() == 0f && height() == 0f;

    // Flips x- and y-axes.
    public void Transpose()
    {
        (top_, left_) = (left_, top_);
        (bottom_, right_) = (right_, bottom_);
    }

    // These setters can be used together with the default constructor and the
    // single-parameter constructor to construct InsetsF instances, for example:
    //                                                    // T, L, B, R
    //   InsetsF a = InsetsF().set_top(2);                // 2, 0, 0, 0
    //   InsetsF b = InsetsF().set_left(2).set_bottom(3); // 0, 2, 3, 0
    //   InsetsF c = InsetsF(1).set_top(5);               // 5, 1, 1, 1

    public InsetsF set_top(float top)
    {
        top_ = top;
        return this;
    }

    public InsetsF set_left(float left)
    {
        left_ = left;
        return this;
    }

    public InsetsF set_bottom(float bottom)
    {
        bottom_ = bottom;
        return this;
    }

    public InsetsF set_right(float right)
    {
        right_ = right;
        return this;
    }

    // In addition to the above, we can also use the following methods to
    // construct InsetsF/OutsetsF.
    // TLBR() is for Chomium UI code. We should not use it in blink code because
    // the order of parameters is different from the normal orders used in blink.
    // Blink code can use the above setters and VH().

    public static InsetsF TLBR(float top, float left, float bottom, float right)
    {
        return new InsetsF(top, left, bottom, right);
    }

    public static InsetsF VH(float vertical, float horizontal)
    {
        return new InsetsF(vertical, horizontal);
    }

    // Sets each side to the maximum of the side and the corresponding side of |other|.
    public void SetToMax(in InsetsF other)
    {
        top_ = Math.Max(top_, other.top_);
        left_ = Math.Max(left_, other.left_);
        bottom_ = Math.Max(bottom_, other.bottom_);
        right_ = Math.Max(right_, other.right_);
    }

    public void Scale(float x_scale, float y_scale)
    {
        top_ *= y_scale;
        left_ *= x_scale;
        bottom_ *= y_scale;
        right_ *= x_scale;
    }

    public void Scale(float scale) => Scale(scale, scale);

    public static InsetsF ScaleInsets(InsetsF i, float x_scale, float y_scale)
    {
        i.Scale(x_scale, y_scale);
        return i;
    }

    public static InsetsF ScaleInsets(InsetsF i, float scale)
    {
        return ScaleInsets(i, scale, scale);
    }
    
    // Conversion from InsetsF to OutsetsF negates all components.
    public OutsetsF ToOutsets() => new OutsetsF(-top(), -left(), -bottom(), -right());

    public override readonly string ToString() => $"x:{left_},{right_} y:{top_},{bottom_}";
    public override readonly int GetHashCode() => HashCode.Combine(top_, left_, bottom_, right_);
    public override readonly bool Equals(object? obj) => obj is InsetsF other && Equals(other);
    public readonly bool Equals(InsetsF other) => top_ == other.top_ && left_ == other.left_ && bottom_ == other.bottom_ && right_ == other.right_;

    public static bool operator ==(in InsetsF left, in InsetsF right) => left.Equals(right);
    public static bool operator !=(in InsetsF left, in InsetsF right) => !left.Equals(right);
    
    public void operator +=(in InsetsF other)
    {
        top_ += other.top_;
        left_ += other.left_;
        bottom_ += other.bottom_;
        right_ += other.right_;
    }

    public void operator -=(in InsetsF other)
    {
        top_ -= other.top_;
        left_ -= other.left_;
        bottom_ -= other.bottom_;
        right_ -= other.right_;
    }

    public static InsetsF operator +(InsetsF lhs, in InsetsF rhs)
    {
        lhs += rhs;
        return lhs;
    }
    
    public static InsetsF operator -(InsetsF lhs, in InsetsF rhs)
    {
        lhs -= rhs;
        return lhs;
    }

    public static InsetsF operator -(in InsetsF v) => new InsetsF(-v.top_, -v.left_, -v.bottom_, -v.right_);
}
