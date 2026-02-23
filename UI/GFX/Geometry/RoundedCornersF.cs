using UI.Extensions;

namespace UI.GFX.Geometry;

// Represents the geometry of a region with rounded corners, expressed as four
// corner radii in the order: top-left, top-right, bottom-right, bottom-left.
public struct RoundedCornersF
{
    private float m_UpperLeft = 0.0f;
    private float m_UpperRight = 0.0f;
    private float m_LowerRight = 0.0f;
    private float m_LowerLeft = 0.0f;

    private static readonly float Trivial = 8.0f * float.MachineEpsilon;

    // Prevents values which are smaller than zero or negligibly small.
    // Uses the same logic as Size.
    static float Clamp(float f) => f > Trivial ? f : 0.0f;

    public float UpperLeft { readonly get => m_UpperLeft; set => m_UpperLeft = Clamp(value); }
    public float UpperRight { readonly get => m_UpperRight; set => m_UpperRight = Clamp(value); }
    public float LowerRight { readonly get => m_LowerRight; set => m_LowerRight = Clamp(value); }
    public float LowerLeft { readonly get => m_LowerLeft; set => m_LowerLeft = Clamp(value); }

    // Creates an empty RoundedCornersF with all corners having zero radius.
    public RoundedCornersF() : this(0.0f) {}

    // Creates a RoundedCornersF with the same radius for all corners.
    public RoundedCornersF(float all) : this(all, all, all, all) {}

    // Creates a RoundedCornersF with four different corner radii.
    public RoundedCornersF(float upper_left, float upper_right, float lower_right, float lower_left)
    {
        m_UpperLeft  = Clamp(upper_left);
        m_UpperRight = Clamp(upper_right);
        m_LowerRight = Clamp(lower_right);
        m_LowerLeft  = Clamp(lower_left);
    }

    public void Set(float upper_left, float upper_right, float lower_right, float lower_left)
    {
        m_UpperLeft = Clamp(upper_left);
        m_UpperRight = Clamp(upper_right);
        m_LowerRight = Clamp(lower_right);
        m_LowerLeft = Clamp(lower_left);
    }

    // Returns true if all of the corners are square (zero effective radius).
    public readonly bool IsEmpty => m_UpperLeft == 0.0f && m_UpperRight == 0.0f && m_LowerRight == 0.0f && m_LowerLeft == 0.0f;

    // Print members in the same order of the constructor parameters.
    public override readonly string ToString() => $"{m_UpperLeft},{m_UpperRight},{m_LowerRight},{m_LowerLeft}";

    public override readonly int GetHashCode() => HashCode.Combine(UpperLeft, UpperRight, LowerRight, LowerLeft);

    public readonly bool Equals(in RoundedCornersF other) => UpperLeft == other.UpperLeft && 
                                                             UpperRight == other.UpperRight &&
                                                             LowerRight == other.LowerRight &&
                                                             LowerLeft == other.LowerLeft;

    public override readonly bool Equals(object? obj) => obj is RoundedCornersF other && Equals(other);

    public static bool operator ==(in RoundedCornersF left, in RoundedCornersF right) => left.Equals(right);
    public static bool operator !=(in RoundedCornersF left, in RoundedCornersF right) => !left.Equals(right);
}
