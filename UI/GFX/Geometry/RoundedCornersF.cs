using UI.Extensions;

namespace UI.GFX.Geometry;

// Represents the geometry of a region with rounded corners, expressed as four
// corner radii in the order: top-left, top-right, bottom-right, bottom-left.
public struct RoundedCornersF
{
    float upper_left_ = 0.0f;
    float upper_right_ = 0.0f;
    float lower_right_ = 0.0f;
    float lower_left_ = 0.0f;

    static float kTrivial = 8.0f * float.MachineEpsilon;

    // Prevents values which are smaller than zero or negligibly small.
    // Uses the same logic as Size.
    static float clamp(float f) => f > kTrivial ? f : 0.0f;

    public float upper_left { readonly get => upper_left_; set => upper_left_ = clamp(upper_left); }
    public float upper_right { readonly get => upper_right_; set => upper_right_ = clamp(upper_right); }
    public float lower_right { readonly get => lower_right_; set => lower_right_ = clamp(lower_right); }
    public float lower_left { readonly get => lower_left_; set => lower_left_ = clamp(lower_left); }

    // Creates an empty RoundedCornersF with all corners having zero radius.
    public RoundedCornersF() : this(0.0f) {}

    // Creates a RoundedCornersF with the same radius for all corners.
    public RoundedCornersF(float all) : this(all, all, all, all) {}

    // Creates a RoundedCornersF with four different corner radii.
    public RoundedCornersF(float upper_left, float upper_right, float lower_right, float lower_left)
    {
        upper_left_ = clamp(upper_left);
        upper_right_ = clamp(upper_right);
        lower_right_ = clamp(lower_right);
        lower_left_ = clamp(lower_left);
    }

    // Returns true if all of the corners are square (zero effective radius).
    public readonly bool IsEmpty => upper_left_ == 0.0f && upper_right_ == 0.0f && lower_right_ == 0.0f && lower_left_ == 0.0f;

    // Print members in the same order of the constructor parameters.
    public override readonly string ToString() => $"{upper_left_},{upper_right_},{lower_right_},{lower_left_}";

    public override readonly int GetHashCode() => HashCode.Combine(upper_left, upper_right, lower_right, lower_left);

    public readonly bool Equals(in RoundedCornersF other) => upper_left == other.upper_left && 
                                                             upper_right == other.upper_right &&
                                                             lower_right == other.lower_right &&
                                                             lower_left == other.lower_left;

    public override readonly bool Equals(object? obj) => obj is RoundedCornersF other && Equals(other);

    public static bool operator ==(in RoundedCornersF left, in RoundedCornersF right) => left.Equals(right);
    public static bool operator !=(in RoundedCornersF left, in RoundedCornersF right) => !left.Equals(right);
}
