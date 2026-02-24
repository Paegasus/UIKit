namespace UI.GFX.Geometry;

/// <summary>
/// A floating point version of Outsets.
/// </summary>
public struct OutsetsF
{
    private float m_Top;
    private float m_Left;
    private float m_Bottom;
    private float m_Right;

    public readonly float Top => m_Top;
    public readonly float Left => m_Left;
    public readonly float Bottom => m_Bottom;
    public readonly float Right => m_Right;

    public OutsetsF(float all) => (m_Top, m_Left, m_Bottom, m_Right) = (all, all, all, all);

    public OutsetsF(float vertical, float horizontal) => (m_Top, m_Left, m_Bottom, m_Right) = (vertical, horizontal, vertical, horizontal);

    public OutsetsF(float top, float left, float bottom, float right) => (m_Top, m_Left, m_Bottom, m_Right) = (top, left, bottom, right);

    // Returns the total width taken up by the insets/outsets, which is the sum of the left and right insets/outsets.
    public readonly float Width => m_Left + m_Right;

    // Returns the total height taken up by the insets/outsets, which is the sum of the top and bottom insets/outsets.
    public readonly float Height => m_Top + m_Bottom;

    // Returns true if the insets/outsets are empty.
    public readonly bool IsEmpty => Width == 0f && Height == 0f;

    // Flips x- and y-axes.
    public void Transpose()
    {
        (m_Top, m_Left) = (m_Left, m_Top);
        (m_Bottom, m_Right) = (m_Right, m_Bottom);
    }

    // These setters can be used together with the default constructor and the
    // single-parameter constructor to construct InsetsF instances, for example:
    //                                                    // T, L, B, R
    //   InsetsF a = InsetsF().set_top(2);                // 2, 0, 0, 0
    //   InsetsF b = InsetsF().set_left(2).set_bottom(3); // 0, 2, 3, 0
    //   InsetsF c = InsetsF(1).set_top(5);               // 5, 1, 1, 1

    public void SetTop(float top)
    {
        m_Top = top;
    }

    public void SetLeft(float left)
    {
        m_Left = left;
    }

    public void SetBottom(float bottom)
    {
        m_Bottom = bottom;
    }

    public void SetRight(float right)
    {
        m_Right = right;
    }

    // In addition to the above, we can also use the following methods to
    // construct InsetsF/OutsetsF.
    // TLBR() is for Chomium UI code. We should not use it in blink code because
    // the order of parameters is different from the normal orders used in blink.
    // Blink code can use the above setters and VH().

    public static OutsetsF TLBR(float top, float left, float bottom, float right) => new OutsetsF(top, left, bottom, right);

    public static OutsetsF VH(float vertical, float horizontal) => new OutsetsF(vertical, horizontal);

    // Sets each side to the maximum of the side and the corresponding side of |other|.
    public void SetToMax(in OutsetsF other)
    {
        m_Top = Math.Max(m_Top, other.m_Top);
        m_Left = Math.Max(m_Left, other.m_Left);
        m_Bottom = Math.Max(m_Bottom, other.m_Bottom);
        m_Right = Math.Max(m_Right, other.m_Right);
    }

    public void Scale(float x_scale, float y_scale)
    {
        m_Top *= y_scale;
        m_Left *= x_scale;
        m_Bottom *= y_scale;
        m_Right *= x_scale;
    }

    public void Scale(float scale) => Scale(scale, scale);

    public readonly InsetsF ToInsets() => new (-Top, -Left, -Bottom, -Right);

    public override readonly string ToString() => $"x:{m_Left},{m_Right} y:{m_Top},{m_Bottom}";
    public override readonly int GetHashCode() => HashCode.Combine(m_Top, m_Left, m_Bottom, m_Right);
    public override readonly bool Equals(object? obj) => obj is OutsetsF other && Equals(other);
    public readonly bool Equals(in OutsetsF other) => m_Top == other.m_Top && m_Left == other.m_Left && m_Bottom == other.m_Bottom && m_Right == other.m_Right;

    public static bool operator ==(in OutsetsF left, in OutsetsF right) => left.Equals(right);
    public static bool operator !=(in OutsetsF left, in OutsetsF right) => !left.Equals(right);

    public void operator +=(in OutsetsF other)
    {
        m_Top += other.m_Top;
        m_Left += other.m_Left;
        m_Bottom += other.m_Bottom;
        m_Right += other.m_Right;
    }

    public void operator -=(in OutsetsF other)
    {
        m_Top -= other.m_Top;
        m_Left -= other.m_Left;
        m_Bottom -= other.m_Bottom;
        m_Right -= other.m_Right;
    }

    public static OutsetsF operator +(OutsetsF a, in OutsetsF b)
    {
        a += b;
        return a;
    }

    public static OutsetsF operator -(OutsetsF a, in OutsetsF b)
    {
        a -= b;
        return a;
    }

    public static OutsetsF operator -(in OutsetsF outsets) => new (-outsets.m_Top, -outsets.m_Left, -outsets.m_Bottom, -outsets.m_Right);
}
