using System.Diagnostics;

namespace UI.GFX.Geometry;

// A 3d version of gfx::RectF, with the positive z-axis pointed towards the camera.
public struct BoxF
{
    private Point3F m_Origin;
    private float m_Width;
    private float m_Height;
    private float m_Depth;

    public Point3F Origin { readonly get => m_Origin; set =>  m_Origin = value; }

    public float X { readonly get => m_Origin.X; set => m_Origin.X = value; }
    public float Y { readonly get => m_Origin.Y; set => m_Origin.Y = value; }
    public float Z { readonly get => m_Origin.Z; set => m_Origin.Z = value; }

    public float Width { readonly get => m_Width; set => m_Width = value < 0 ? 0 : value; }
    public float Height { readonly get => m_Height; set => m_Height = value < 0 ? 0 : value; }
    public float Depth { readonly get => m_Depth; set => m_Depth = value < 0 ? 0 : value; }

    public readonly float Right => X + Width;
    public readonly float Bottom => Y + Height;
    public readonly float Front => Z + Depth;

    public BoxF() : this(0f, 0f, 0f, 0f, 0f, 0f) { }
    public BoxF(float width, float height, float depth) : this(0f, 0f, 0f, width, height, depth) { }
    public BoxF(float x, float y, float z, float width, float height, float depth) : this(new Point3F(x, y, z), width, height, depth) { }
    public BoxF(Point3F origin, float width, float height, float depth)
    {
        m_Origin = origin;
        m_Width = width >= 0f ? width : 0f;
        m_Height = height >= 0f ? height : 0f;
        m_Depth = depth >= 0f ? depth : 0f;
    }

    // Scales all three axes by the given scale.
    public void Scale(float scale) => Scale(scale, scale, scale);

    // Scales each axis by the corresponding given scale.
    public void Scale(float x_scale, float y_scale, float z_scale)
    {
        m_Origin.Scale(x_scale, y_scale, z_scale);
        SetSize(m_Width * x_scale, m_Height * y_scale, m_Depth * z_scale);
    }

    // Returns true if the box has no interior points.
    public readonly bool IsEmpty => (m_Width == 0f && m_Height == 0f) || (m_Width == 0f && m_Depth == 0f) || (m_Height == 0f && m_Depth == 0f);

    // Computes the union of this box with the given box. The union is the smallest box that contains both boxes.
    public void Union(in BoxF box)
    {
        if (IsEmpty)
        {
            this = box;
            return;
        }
        if (box.IsEmpty)
            return;
        
        ExpandTo(box);
    }

    public override readonly string ToString() => $"{m_Origin:G6} {m_Width:G6}x{m_Height:G6}x{m_Depth:G6}";

    public void SetSize(float width, float height, float depth)
    {
        m_Width = width < 0f ? 0f : width;
        m_Height = height < 0f ? 0f : height;
        m_Depth = depth < 0f ? 0f : depth;
    }

    // Expands |this| to contain the given point, if necessary. Please note, even
    // if |this| is empty, after the function |this| will continue to contain its |origin_|.
    public void ExpandTo(in Point3F point) => ExpandTo(point, point);

    // Expands |this| to contain the given box, if necessary. Please note, even
    // if |this| is empty, after the function |this| will continue to contain its |origin_|.
    public void ExpandTo(in BoxF box) => ExpandTo(box.m_Origin, new Point3F(box.Right, box.Bottom, box.Front));

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
        float min_x = Math.Min(X, min.X);
        float min_y = Math.Min(Y, min.Y);
        float min_z = Math.Min(Z, min.Z);
        float max_x = Math.Max(Right, max.X);
        float max_y = Math.Max(Bottom, max.Y);
        float max_z = Math.Max(Front, max.Z);

        m_Origin.SetPoint(min_x, min_y, min_z);
        m_Width = max_x - min_x;
        m_Height = max_y - min_y;
        m_Depth = max_z - min_z;
    }

    public override readonly int GetHashCode() => HashCode.Combine(m_Origin, m_Width, m_Height, m_Depth);

    public override readonly bool Equals(object? obj) => obj is BoxF other && Equals(other);

    public readonly bool Equals(in BoxF other) => m_Origin.Equals(other.m_Origin) && m_Width == other.m_Width && m_Height == other.m_Height && m_Depth == other.m_Depth;

    public static bool operator ==(in BoxF left, in BoxF right) => left.Equals(right);
    public static bool operator !=(in BoxF left, in BoxF right) => !left.Equals(right);

    public static BoxF operator +(in BoxF a, in Vector3DF b) => new (a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.Width, a.Height, a.Depth);

    // Moves the box by the specified distance in each dimension.
    public void operator +=(in Vector3DF offset) => m_Origin += offset;

    public static BoxF UnionBoxes(in BoxF a, in BoxF b)
    {
        BoxF result = a;
        result.Union(b);
        return result;
    }

    public static BoxF ScaleBox(in BoxF box, float x_scale, float y_scale, float z_scale) => new (box.X * x_scale, box.Y * y_scale, box.Z * z_scale, box.Width * x_scale, box.Height * y_scale, box.Depth * z_scale);

    public static BoxF ScaleBox(in BoxF box, float scale) => ScaleBox(box, scale, scale, scale);
}
