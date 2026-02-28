using System.Diagnostics;

namespace UI.GFX.Geometry;

public struct MaskFilterInfo
{
    // The rounded corner bounds. This also defines the bounds that the mask
    // filter will be applied to.
    RRectF rounded_corner_bounds_;

    // Shader based linear gradient mask to be applied to a layer.
    LinearGradient? gradient_mask_;

    public int? clip_id_;

    public MaskFilterInfo(in RRectF rrect)
    {
        rounded_corner_bounds_ = rrect;
    }

    public MaskFilterInfo(in RRectF rrect, in LinearGradient gradient_mask)
    {
        rounded_corner_bounds_ = rrect;
        gradient_mask_ = gradient_mask;
    }

    public MaskFilterInfo(in RectF bounds, in RoundedCornersF radii, in LinearGradient gradient_mask)
    {
        rounded_corner_bounds_ = new(bounds, radii);
        gradient_mask_ = gradient_mask;
    }

    // The bounds the filter will be applied to.
    public readonly RectF bounds() => rounded_corner_bounds_.rect();

    // Defines the rounded corner bounds to clip.
    // Note: Original function returns a RRectF&
    public readonly RRectF rounded_corner_bounds() => rounded_corner_bounds_;

    // True if this contains a rounded corner mask.
    public readonly bool HasRoundedCorners() => rounded_corner_bounds_.HasRoundedCorners();

    // Note: Original funciton returns a LinearGradient&
    public readonly LinearGradient? gradient_mask() => gradient_mask_;

    // True if this contains an effective gradient mask (requires filter bounds).
    public readonly bool HasGradientMask()
    {
        if (rounded_corner_bounds_.IsEmpty)
            return false;

        return gradient_mask_ != null && !gradient_mask_.IsEmpty;
    }

    // True if this contains no effective mask information.
    public readonly bool IsEmpty() => rounded_corner_bounds_.IsEmpty;

    // Transform the mask filter information. If the transform cannot be applied
    // (e.g. it would make rounded_corner_bounds_ invalid), rounded_corner_bounds_
    // will be set to empty.
    public void ApplyTransform(in Transform transform)
    {

    }

    public void ApplyTransform(in AxisTransform2D transform)
    {
        
    }

    public override string ToString()
    {
        
    }

    public override readonly int GetHashCode() => HashCode.Combine(rounded_corner_bounds_, gradient_mask_, clip_id_);

    public readonly bool Equals(in MaskFilterInfo other) => rounded_corner_bounds_ == other.rounded_corner_bounds_ &&
                                                            gradient_mask_ == other.gradient_mask_ &&
                                                            clip_id_ == other.clip_id_;
        
    public override readonly bool Equals(object? obj) => obj is MaskFilterInfo other && Equals(other);

    public static bool operator ==(in MaskFilterInfo left, in MaskFilterInfo right) => left.Equals(right);
    public static bool operator !=(in MaskFilterInfo left, in MaskFilterInfo right) => !left.Equals(right);
}
