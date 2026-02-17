using System;
using System.Diagnostics;

namespace UI.GFX.Geometry;

// A 3d version of gfx::RectF, with the positive z-axis pointed towards the camera.
public struct BoxF : IEquatable<BoxF>
{
    private Point3F origin_;
    private float width_;
    private float height_;
    private float depth_;

    public BoxF() : this(0f, 0f, 0f) { }
    public BoxF(float width, float height, float depth) : this(0f, 0f, 0f, width, height, depth) { }
    public BoxF(float x, float y, float z, float width, float height, float depth) : this(new Point3F(x, y, z), width, height, depth) { }
    public BoxF(Point3F origin, float width, float height, float depth)
    {
        origin_ = origin;
        width_ = width >= 0f ? width : 0f;
        height_ = height >= 0f ? height : 0f;
        depth_ = depth >= 0f ? depth : 0f;
    }

    // Scales all three axes by the given scale.
    public void Scale(float scale) => Scale(scale, scale, scale);

    // Scales each axis by the corresponding given scale.
    public void Scale(float x_scale, float y_scale, float z_scale)
    {
        origin_.Scale(x_scale, y_scale, z_scale);
        set_size(width_ * x_scale, height_ * y_scale, depth_ * z_scale);
    }

    // Returns true if the box has no interior points.
    public readonly bool IsEmpty() => (width_ == 0f && height_ == 0f) || (width_ == 0f && depth_ == 0f) || (height_ == 0f && depth_ == 0f);

    // Computes the union of this box with the given box. The union is the smallest box that contains both boxes.
    public void Union(in BoxF box)
    {
        if (IsEmpty())
        {
            this = box;
            return;
        }
        if (box.IsEmpty())
            return;
        ExpandTo(box);
    }

    public override readonly string ToString() => $"{origin_} {width_}x{height_}x{depth_}";

    public float x { readonly get => origin_.X; set => origin_.X = value; }
    public float y { readonly get => origin_.Y; set => origin_.Y = value; }
    public float z { readonly get => origin_.Z; set => origin_.Z = value; }

    public float width { readonly get => width_; set => width_ = value < 0 ? 0 : value; }
    public float height { readonly get => height_; set => height_ = value < 0 ? 0 : value; }
    public float depth { readonly get => depth_; set => depth_ = value < 0 ? 0 : value; }

    public readonly float right() => x + width;
    public readonly float bottom() => y + height;
    public readonly float front() => z + depth;

    public void set_size(float width, float height, float depth)
    {
        width_ = width < 0f ? 0f : width;
        height_ = height < 0f ? 0f : height;
        depth_ = depth < 0f ? 0f : depth;
    }

    public readonly Point3F origin() => origin_;
    
    public void set_origin(Point3F origin) { origin_ = origin; }

    // Expands |this| to contain the given point, if necessary. Please note, even
    // if |this| is empty, after the function |this| will continue to contain its |origin_|.
    public void ExpandTo(in Point3F point) => ExpandTo(point, point);

    // Expands |this| to contain the given box, if necessary. Please note, even
    // if |this| is empty, after the function |this| will continue to contain its |origin_|.
    public void ExpandTo(in BoxF box) => ExpandTo(box.origin_, new Point3F(box.right(), box.bottom(), box.front()));

    // Expands the box to contain the two given points. It is required that each
    // component of |min| is less than or equal to the corresponding component in
    // |max|. Precisely, what this function does is ensure that after the function
    // completes, |this| contains origin_, min, max, and origin_ + (width_,
    // height_, depth_), even if the box is empty. Emptiness checks are handled in
    // the public function Union.
    private void ExpandTo(in Point3F min, in Point3F max)
    {
#if DEBUG
        Debug.Assert(min.X <= max.X);
        Debug.Assert(min.Y <= max.Y);
        Debug.Assert(min.Z <= max.Z);
#endif
        float min_x = Math.Min(x, min.X);
        float min_y = Math.Min(y, min.Y);
        float min_z = Math.Min(z, min.Z);
        float max_x = Math.Max(right(), max.X);
        float max_y = Math.Max(bottom(), max.Y);
        float max_z = Math.Max(front(), max.Z);

        origin_.SetPoint(min_x, min_y, min_z);
        width_ = max_x - min_x;
        height_ = max_y - min_y;
        depth_ = max_z - min_z;
    }

    public override readonly int GetHashCode() => HashCode.Combine(origin_, width_, height_, depth_);

    public override readonly bool Equals(object? obj) => obj is BoxF other && Equals(other);

    public readonly bool Equals(BoxF other) => origin_.Equals(other.origin_) && width_ == other.width_ && height_ == other.height_ && depth_ == other.depth_;

    public static bool operator ==(in BoxF lhs, in BoxF rhs) => lhs.Equals(rhs);
    public static bool operator !=(in BoxF lhs, in BoxF rhs) => !lhs.Equals(rhs);

    public static BoxF operator +(in BoxF b, in Vector3DF v) => new BoxF(b.x + v.X, b.y + v.Y, b.z + v.Z, b.width, b.height, b.depth);

    // Moves the box by the specified distance in each dimension.
    public void operator +=(in Vector3DF offset)
    {
        origin_ += offset;
    }

    public static BoxF UnionBoxes(in BoxF a, in BoxF b)
    {
        BoxF result = a;
        result.Union(b);
        return result;
    }

    public static BoxF ScaleBox(in BoxF b, float x_scale, float y_scale, float z_scale) => new BoxF(b.x * x_scale, b.y * y_scale, b.z * z_scale, b.width * x_scale, b.height * y_scale, b.depth * z_scale);

    public static BoxF ScaleBox(in BoxF b, float scale) => ScaleBox(b, scale, scale, scale);
}
