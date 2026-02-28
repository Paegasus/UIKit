using System.Diagnostics;
using SkiaSharp;

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

        return gradient_mask_ != null && !gradient_mask_.Value.IsEmpty;
    }

    // True if this contains no effective mask information.
    public readonly bool IsEmpty() => rounded_corner_bounds_.IsEmpty;

    /*
    public void ApplyTransform(in Transform transform)
    {
        if (rounded_corner_bounds_.IsEmpty)
            return;

        if (!transform.Preserves2dAxisAlignment())
        {
            rounded_corner_bounds_ = new RRectF();
            return;
        }

        float kEpsilon = float.Epsilon;

        // Get the flattened 2D matrix elements.
        float scaleX = (float)transform.rc(0, 0);
        //float skewX = (float)transform.rc(0, 1);
        //float skewY = (float)transform.rc(1, 0);
        float skewX = (float)transform.rc(1, 0);
        float skewY = (float)transform.rc(0, 1);
        float scaleY = (float)transform.rc(1, 1);
        float transX = (float)transform.rc(0, 3);
        float transY = (float)transform.rc(1, 3);

        // Round near-zero values to zero, matching the C++ epsilon rounding.
        if (MathF.Abs(scaleX) < kEpsilon) scaleX = 0f;
        if (MathF.Abs(skewX) < kEpsilon) skewX = 0f;
        if (MathF.Abs(skewY) < kEpsilon) skewY = 0f;
        if (MathF.Abs(scaleY) < kEpsilon) scaleY = 0f;

        // For axis-aligned transforms, either (scaleX, scaleY) or (skewX, skewY)
        // are non-zero, but not both. A 90-degree rotation swaps axes.
        bool swapAxes = scaleX == 0f && scaleY == 0f;

        SKRect oldRect = rounded_corner_bounds_._rect;

        // Map the bounding rect.
        SKRect newRect;

        //if (swapAxes)
        //{
        //    // 90/270 degree rotation: x maps to y and vice versa.
        //    newRect = new SKRect(
        //        oldRect.Left * skewX + transX,
        //        oldRect.Top * skewY + transY,
        //        oldRect.Right * skewX + transX,
        //        oldRect.Bottom * skewY + transY);
        //}

        if (swapAxes)
        {
            // 90/270 degree rotation: x maps to y and vice versa.
            float newLeft = MathF.Min(skewX * oldRect.Top, skewX * oldRect.Bottom) + transX;
            float newRight = MathF.Max(skewX * oldRect.Top, skewX * oldRect.Bottom) + transX;
            float newTop = MathF.Min(skewY * oldRect.Left, skewY * oldRect.Right) + transY;
            float newBottom = MathF.Max(skewY * oldRect.Left, skewY * oldRect.Right) + transY;
            newRect = new SKRect(newLeft, newTop, newRight, newBottom);
        }
        else
        {
            newRect = new SKRect(
                oldRect.Left * scaleX + transX,
                oldRect.Top * scaleY + transY,
                oldRect.Right * scaleX + transX,
                oldRect.Bottom * scaleY + transY);
        }

        // makeSorted equivalent — ensure left <= right, top <= bottom.
        newRect = new SKRect(
            MathF.Min(newRect.Left, newRect.Right),
            MathF.Min(newRect.Top, newRect.Bottom),
            MathF.Max(newRect.Left, newRect.Right),
            MathF.Max(newRect.Top, newRect.Bottom));

        if (!float.IsFinite(newRect.Left) || !float.IsFinite(newRect.Top) ||
            !float.IsFinite(newRect.Right) || !float.IsFinite(newRect.Bottom))
        {
            rounded_corner_bounds_ = new RRectF();
            return;
        }

        // Scale radii — swap x/y components for 90-degree rotations.
        SKPoint ScaleRadii(SKPoint r) => swapAxes
            ? new SKPoint(MathF.Abs(r.Y * skewX), MathF.Abs(r.X * skewY))
            : new SKPoint(MathF.Abs(r.X * scaleX), MathF.Abs(r.Y * scaleY));

        rounded_corner_bounds_._rect = newRect;
        rounded_corner_bounds_._radiiUpperLeft = ScaleRadii(rounded_corner_bounds_._radiiUpperLeft);
        rounded_corner_bounds_._radiiUpperRight = ScaleRadii(rounded_corner_bounds_._radiiUpperRight);
        rounded_corner_bounds_._radiiLowerRight = ScaleRadii(rounded_corner_bounds_._radiiLowerRight);
        rounded_corner_bounds_._radiiLowerLeft = ScaleRadii(rounded_corner_bounds_._radiiLowerLeft);

        rounded_corner_bounds_.Normalize();

        //if (gradient_mask_ != null && !gradient_mask_.Value.IsEmpty)
        //    gradient_mask_.Value.ApplyTransform(transform);

        if (gradient_mask_.HasValue && !gradient_mask_.Value.IsEmpty)
        {
            var g = gradient_mask_.Value;
            g.ApplyTransform(transform);
            gradient_mask_ = g;
        }

        Debug.WriteLine($"scaleX={scaleX} skewX={skewX} skewY={skewY} scaleY={scaleY} transX={transX} transY={transY}");
        Debug.WriteLine($"swapAxes={swapAxes}");
    }
    */

    // Transform the mask filter information. If the transform cannot be applied
    // (e.g. it would make rounded_corner_bounds_ invalid), rounded_corner_bounds_
    // will be set to empty.
    public void ApplyTransform(in Transform transform)
    {
        if (rounded_corner_bounds_.IsEmpty)
            return;

        if (!transform.Preserves2dAxisAlignment())
        {
            rounded_corner_bounds_ = new RRectF();
            return;
        }

        float kEpsilon = float.Epsilon;

        // Get the flattened 2D matrix elements.
        float scaleX = (float)transform.rc(0, 0);
        // NOTE: Indices are intentionally swapped here. Matrix44.rc(row, col) internally
        // returns this[col, row] to compensate for SkiaSharp's column-major storage order,
        // which means the effective (row, col) access is transposed for full matrices.
        // rc(1, 0) retrieves r0c1 (skewX) and rc(0, 1) retrieves r1c0 (skewY).
        float skewX = (float)transform.rc(1, 0);
        float skewY = (float)transform.rc(0, 1);
        float scaleY = (float)transform.rc(1, 1);
        float transX = (float)transform.rc(0, 3);
        float transY = (float)transform.rc(1, 3);

        // Round near-zero values to zero, matching the C++ epsilon rounding.
        if (MathF.Abs(scaleX) < kEpsilon) scaleX = 0f;
        if (MathF.Abs(skewX) < kEpsilon) skewX = 0f;
        if (MathF.Abs(skewY) < kEpsilon) skewY = 0f;
        if (MathF.Abs(scaleY) < kEpsilon) scaleY = 0f;

        // For axis-aligned transforms, either (scaleX, scaleY) or (skewX, skewY)
        // are non-zero, but not both. A 90-degree rotation swaps axes.
        bool swapAxes = scaleX == 0f && scaleY == 0f;

        SKRect oldRect = rounded_corner_bounds_._rect;

        // Map the bounding rect.
        SKRect newRect;
        if (swapAxes)
        {
            // 90/270 degree rotation: x maps to y and vice versa.
            float newLeft = MathF.Min(skewX * oldRect.Top, skewX * oldRect.Bottom) + transX;
            float newRight = MathF.Max(skewX * oldRect.Top, skewX * oldRect.Bottom) + transX;
            float newTop = MathF.Min(skewY * oldRect.Left, skewY * oldRect.Right) + transY;
            float newBottom = MathF.Max(skewY * oldRect.Left, skewY * oldRect.Right) + transY;
            newRect = new SKRect(newLeft, newTop, newRight, newBottom);
        }
        else
        {
            newRect = new SKRect(
                oldRect.Left * scaleX + transX,
                oldRect.Top * scaleY + transY,
                oldRect.Right * scaleX + transX,
                oldRect.Bottom * scaleY + transY);
        }

        // makeSorted equivalent — ensure left <= right, top <= bottom.
        newRect = new SKRect(
            MathF.Min(newRect.Left, newRect.Right),
            MathF.Min(newRect.Top, newRect.Bottom),
            MathF.Max(newRect.Left, newRect.Right),
            MathF.Max(newRect.Top, newRect.Bottom));

        if (!float.IsFinite(newRect.Left) || !float.IsFinite(newRect.Top) ||
            !float.IsFinite(newRect.Right) || !float.IsFinite(newRect.Bottom))
        {
            rounded_corner_bounds_ = new RRectF();
            return;
        }

        // Scale radii — swap x/y components for 90-degree rotations.
        SKPoint ScaleRadii(SKPoint r) => swapAxes
            ? new SKPoint(MathF.Abs(r.Y * skewX), MathF.Abs(r.X * skewY))
            : new SKPoint(MathF.Abs(r.X * scaleX), MathF.Abs(r.Y * scaleY));

        // Snapshot old radii before any writes.
        var oldUL = rounded_corner_bounds_._radiiUpperLeft;
        var oldUR = rounded_corner_bounds_._radiiUpperRight;
        var oldLR = rounded_corner_bounds_._radiiLowerRight;
        var oldLL = rounded_corner_bounds_._radiiLowerLeft;

        rounded_corner_bounds_._rect = newRect;

        if (swapAxes)
        {
            if (skewX < 0)
            {
                // 90° CCW rotation: corners rotate CCW.
                rounded_corner_bounds_._radiiUpperLeft = ScaleRadii(oldLL);
                rounded_corner_bounds_._radiiUpperRight = ScaleRadii(oldUL);
                rounded_corner_bounds_._radiiLowerRight = ScaleRadii(oldUR);
                rounded_corner_bounds_._radiiLowerLeft = ScaleRadii(oldLR);
            }
            else
            {
                // 90° CW rotation: corners rotate CW.
                rounded_corner_bounds_._radiiUpperLeft = ScaleRadii(oldUR);
                rounded_corner_bounds_._radiiUpperRight = ScaleRadii(oldLR);
                rounded_corner_bounds_._radiiLowerRight = ScaleRadii(oldLL);
                rounded_corner_bounds_._radiiLowerLeft = ScaleRadii(oldUL);
            }
        }
        else
        {
            rounded_corner_bounds_._radiiUpperLeft = ScaleRadii(oldUL);
            rounded_corner_bounds_._radiiUpperRight = ScaleRadii(oldUR);
            rounded_corner_bounds_._radiiLowerRight = ScaleRadii(oldLR);
            rounded_corner_bounds_._radiiLowerLeft = ScaleRadii(oldLL);
        }

        rounded_corner_bounds_.Normalize();

        if (gradient_mask_.HasValue && !gradient_mask_.Value.IsEmpty)
        {
            var g = gradient_mask_.Value;
            g.ApplyTransform(transform);
            gradient_mask_ = g;
        }
    }

    public void ApplyTransform(in AxisTransform2D transform)
    {
        if (rounded_corner_bounds_.IsEmpty)
            return;

        rounded_corner_bounds_.Scale(transform.Scale.X, transform.Scale.Y);
        rounded_corner_bounds_.Offset(transform.Translation);

        if (!rounded_corner_bounds_.IsValid())
        {
            rounded_corner_bounds_ = new RRectF();
            return;
        }

        if (gradient_mask_ != null && !gradient_mask_.Value.IsEmpty)
            gradient_mask_.Value.ApplyTransform(transform);
    }

    public override string ToString()
    {
        string result = "MaskFilterInfo{" + rounded_corner_bounds_.ToString();

        if (gradient_mask_ != null)
        {
            result += ", gradient_mask=" + gradient_mask_.ToString();
        }

        if (clip_id_.HasValue)
        {
            result += ", clip_id=" + clip_id_.Value.ToString();
        }

        result += "}";

        return result;
    }

    public override readonly int GetHashCode() => HashCode.Combine(rounded_corner_bounds_, gradient_mask_, clip_id_);

    public readonly bool Equals(in MaskFilterInfo other) =>
                                                            rounded_corner_bounds_ == other.rounded_corner_bounds_ &&
                                                            clip_id_               == other.clip_id_               &&
                                                            gradient_mask_.HasValue == other.gradient_mask_.HasValue &&
                                                            (!gradient_mask_.HasValue || gradient_mask_.Value.Equals(other.gradient_mask_.Value));
        
    public override readonly bool Equals(object? obj) => obj is MaskFilterInfo other && Equals(other);

    public static bool operator ==(in MaskFilterInfo left, in MaskFilterInfo right) => left.Equals(right);
    public static bool operator !=(in MaskFilterInfo left, in MaskFilterInfo right) => !left.Equals(right);
}
