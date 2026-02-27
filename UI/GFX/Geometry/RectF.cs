using System.Diagnostics;
using UI.Extensions;

namespace UI.GFX.Geometry;

// A floating version of Rect.
public struct RectF
{
    private PointF m_Origin;
    private SizeF m_Size;

    public RectF(float x, float y, float width, float height) 
    {
        m_Origin = new PointF(x, y);
        m_Size = new SizeF(width, height);
    }

    public RectF(float width, float height) : this(0, 0, width, height) { }

    public RectF(in SizeF size) : this(0, 0, size.Width, size.Height) { }
    
    public RectF(in PointF origin, in SizeF size) : this(origin.X, origin.Y, size.Width, size.Height) { }
    public RectF(in Rect r) : this(r.X, r.Y, r.Width, r.Height) { }

    public float X { readonly get => m_Origin.X; set => m_Origin.X = value; }
    public float Y { readonly get => m_Origin.Y; set => m_Origin.Y = value; }
    public float Width { readonly get => m_Size.Width; set => m_Size.Width = value; }
    public float Height { readonly get => m_Size.Height; set => m_Size.Height = value; }

    public PointF Origin { readonly get => m_Origin; set => m_Origin = value; }

    public  SizeF Size { readonly get => m_Size; set => m_Size = value; }

    public readonly float Right => X + Width;
    public readonly float Bottom => Y + Height;

    public readonly PointF TopRight => new(Right, Y);
    public readonly PointF BottomLeft => new(X, Bottom);
    public readonly PointF BottomRight => new(Right, Bottom);

    public readonly PointF LeftCenter => new(X, Y + Height / 2);
    public readonly PointF TopCenter => new(X + Width / 2, Y);
    public readonly PointF RightCenter => new(Right, Y + Height / 2);
    public readonly PointF BottomCenter => new(X + Width / 2, Bottom);

    public readonly Vector2DF OffsetFromOrigin() => new(X, Y);

    public void SetRect(float x, float y, float width, float height)
    {
        m_Origin.SetPoint(x, y);
        m_Size.SetSize(width, height);
    }

    // Shrinks the rectangle by |inset| on all sides.
    public void Inset(float inset) => Inset(new InsetsF(inset));

    // Shrinks the rectangle by the given |insets|.
    public void Inset(in InsetsF insets)
    {
        m_Origin.Offset(insets.Left, insets.Top);
        Width -= insets.Width;
        Height -= insets.Height;
    }

    // Expands the rectangle by |outset| on all sides.
    public void Outset(float outset) => Inset(-outset);

    // Expands the rectangle by the given |outsets|.
    public void Outset(in OutsetsF outsets) => Inset(outsets.ToInsets());

    // Move the rectangle by a horizontal and vertical distance.
    public void Offset(float horizontal, float vertical) => m_Origin.Offset(horizontal, vertical);
    public void Offset(in Vector2DF distance) => m_Origin.Offset(distance.X, distance.Y);

    public readonly InsetsF InsetsFrom(in RectF inner) => InsetsF.TLBR(inner.Y - Y, inner.X - X, Bottom - inner.Bottom, Right - inner.Right);

    // Returns true if the area of the rectangle is zero.
    public readonly bool IsEmpty() => m_Size.IsEmpty;

    // Returns true if the point identified by point_x and point_y falls inside
    // this rectangle (including the left and the top edges, excluding the right
    // and the bottom edges). If this rectangle is empty, this method returns
    // false regardless of the point.
    public readonly bool Contains(float point_x, float point_y) => point_x >= X && point_x < Right && point_y >= Y && point_y < Bottom;

    // Returns true if the specified point is contained by this rectangle.
    public readonly bool Contains(in PointF point) => Contains(point.X, point.Y);
    
    // Returns true if this rectangle contains the specified rectangle.
    public readonly bool Contains(in RectF rect) => rect.X >= X && rect.Right <= Right && rect.Y >= Y && rect.Bottom <= Bottom;
    
    // Similar to Contains(), but uses edge-inclusive geometry, i.e. also returns
    // true if the point is on the right or the bottom edge. If this rectangle
    // is empty, this method returns true only if the point is at the origin of
    // this rectangle.
    public readonly bool InclusiveContains(float point_x, float point_y) => point_x >= X && point_x <= Right && point_y >= Y && point_y <= Bottom;

    public readonly bool InclusiveContains(in PointF point) => InclusiveContains(point.X, point.Y);

    // Returns true if this rectangle intersects the specified rectangle.
    // An empty rectangle doesn't intersect any rectangle.
    public readonly bool Intersects(in RectF rect) => !IsEmpty() && !rect.IsEmpty() && rect.X < Right && rect.Right > X && rect.Y < Bottom && rect.Bottom > Y;

    // Sets this rect to be the intersection of this rectangle with the given rectangle.
    public void Intersect(in RectF rect)
    {
        if (IsEmpty() || rect.IsEmpty())
        {
            SetRect(0, 0, 0, 0);
            return;
        }

        float rx = Math.Max(X, rect.X);
        float ry = Math.Max(Y, rect.Y);
        float rr = Math.Min(Right, rect.Right);
        float rb = Math.Min(Bottom, rect.Bottom);
        
        if (rx >= rr || ry >= rb)
        {
            SetRect(0, 0, 0, 0);
            return;
        }

        SetRect(rx, ry, rr - rx, rb - ry);
    }

    // Sets this rect to be the intersection of itself and |rect| using
    // edge-inclusive geometry.  If the two rectangles overlap but the overlap
    // region is zero-area (either because one of the two rectangles is zero-area,
    // or because the rectangles overlap at an edge or a corner), the result is
    // the zero-area intersection.  The return value indicates whether the two
    // rectangle actually have an intersection, since checking the result for
    // isEmpty() is not conclusive.
    public bool InclusiveIntersect(in RectF rect)
    {
        float rx = Math.Max(X, rect.X);
        float ry = Math.Max(Y, rect.Y);
        float rr = Math.Min(Right, rect.Right);
        float rb = Math.Min(Bottom, rect.Bottom);

        // Return a clean empty rectangle for non-intersecting cases.
        if (rx > rr || ry > rb)
        {
            SetRect(0, 0, 0, 0);
            return false;
        }

        SetRect(rx, ry, rr - rx, rb - ry);
        return true;
    }

    // Sets this rect to be the union of this rectangle with the given rectangle.
    // The union is the smallest rectangle containing both rectangles if not
    // empty. If both rects are empty, this rect will become |rect|.
    public void Union(in RectF rect)
    {
        if (IsEmpty())
        {
            this = rect;
            return;
        }
        if (rect.IsEmpty())
            return;

        UnionEvenIfEmpty(rect);
    }

    // Similar to Union(), but the result will contain both rectangles even if
    // either of them is empty. For example, union of (100, 100, 0x0) and
    // (200, 200, 50x0) is (100, 100, 150x100).
    public void UnionEvenIfEmpty(in RectF rect)
    {
        float rx = Math.Min(X, rect.X);
        float ry = Math.Min(Y, rect.Y);
        float rr = Math.Max(Right, rect.Right);
        float rb = Math.Max(Bottom, rect.Bottom);
        
        SetRect(rx, ry, rr - rx, rb - ry);

        // Due to floating errors and SizeF::clamp(), the new rect may not fully
        // contain the original rects at the right/bottom side. Expand the rect in the case.

        if (Right < rr && Width < float.MaxValue)
        
            m_Size.SetToNextWidth();
#if DEBUG
            Debug.Assert(Right >= rr, "RectF.UnionEvenIfEmpty(): right() should be >= rr.");
#endif
        
        if (Bottom < rb && Height < float.MaxValue)
        {
            m_Size.SetToNextHeight();
#if DEBUG
            Debug.Assert(Bottom >= rb, "RectF.UnionEvenIfEmpty(): bottom() should be >= rb.");
#endif
        }
    }

    // Sets this rect to be the rectangle resulting from subtracting |rect| from
    // |*this|, i.e. the bounding rect of |Region(*this) - Region(rect)|.
    public void Subtract(in RectF rect)
    {
        if (!Intersects(rect))
            return;

        if (rect.Contains(this))
        {
            SetRect(0, 0, 0, 0);
            return;
        }

        float rx = X;
        float ry = Y;
        float rr = Right;
        float rb = Bottom;

        if (rect.Y <= Y && rect.Bottom >= Bottom)
        {
            // complete intersection in the y-direction

            if (rect.X <= X)
                rx = rect.Right;

            else if (rect.Right >= Right)
                rr = rect.X;
        }
        else if (rect.X <= X && rect.Right >= Right)
        {
            // complete intersection in the x-direction

            if (rect.Y <= Y)
                ry = rect.Bottom;

            else if (rect.Bottom >= Bottom)
                rb = rect.Y;
        }

        SetRect(rx, ry, rr - rx, rb - ry);
    }

    // Fits as much of the receiving rectangle into the supplied rectangle as
    // possible, becoming the result. For example, if the receiver had
    // a x-location of 2 and a width of 4, and the supplied rectangle had
    // an x-location of 0 with a width of 5, the returned rectangle would have
    // an x-location of 1 with a width of 4.
    public void AdjustToFit(in RectF rect)
    {
        float new_x = X;
        float new_y = Y;
        float new_width = Width;
        float new_height = Height;
        AdjustAlongAxis(rect.X, rect.Width, ref new_x, ref new_width);
        AdjustAlongAxis(rect.Y, rect.Height, ref new_y, ref new_height);
        SetRect(new_x, new_y, new_width, new_height);
    }

    // Returns the center of this rectangle.
    public readonly PointF CenterPoint() => new(X + Width / 2, Y + Height / 2);

    // Becomes a rectangle that has the same center point but with a size capped at given |size|.
    public void ClampToCenteredSize(in SizeF size)
    {
        float new_width = Math.Min(Width, size.Width);
        float new_height = Math.Min(Height, size.Height);
        float new_x = X + (Width - new_width) / 2;
        float new_y = Y + (Height - new_height) / 2;
        SetRect(new_x, new_y, new_width, new_height);
    }

    // Transpose x and y axis.
    public void Transpose() => SetRect(Y, X, Height, Width);

    // Splits `this` in two halves, `left_half` and `right_half`.
    public void SplitVertically(out RectF left_half, out RectF right_half)
    {
        left_half = new RectF(X, Y, Width / 2, Height);
        right_half = new RectF(left_half.Right, Y, Width - left_half.Width, Height);
    }

    // Splits `this` in two halves, `top_half` and `bottom_half`.
    public void SplitHorizontally(out RectF top_half, out RectF bottom_half)
    {
        top_half = new RectF(X, Y, Width, Height / 2);
        bottom_half = new RectF(X, top_half.Bottom, Width, Height - top_half.Height);
    }

    // Returns true if this rectangle shares an entire edge (i.e., same width or
    // same height) with the given rectangle, and the rectangles do not overlap.
    public readonly bool SharesEdgeWith(in RectF rect) =>
        (Y == rect.Y && Height == rect.Height && (X == rect.Right || Right == rect.X)) ||
        (X == rect.X && Width == rect.Width && (Y == rect.Bottom || Bottom == rect.Y));

    // Returns the manhattan distance from the rect to the point. If the point is
    // inside the rect, returns 0.
    public readonly float ManhattanDistanceToPoint(in PointF point) =>
        Math.Max(0, Math.Max(X - point.X, point.X - Right)) +
        Math.Max(0, Math.Max(Y - point.Y, point.Y - Bottom));

    // Returns the manhattan distance between the contents of this rect and the
    // contents of the given rect. That is, if the intersection of the two rects
    // is non-empty then the function returns 0. If the rects share a side, it
    // returns the smallest non-zero value appropriate for float.
    public float ManhattanInternalDistance(in RectF rect)
    {
        RectF c = this;
        c.Union(rect);
        
        float x = Math.Max(0, c.Width - Width - rect.Width + float.MachineEpsilon);
        float y = Math.Max(0, c.Height - Height - rect.Height + float.MachineEpsilon);
        return x + y;
    }

    // Returns the closest point in or on an edge of this rect to the given point.
    public readonly PointF ClosestPoint(in PointF point)
    {
        // Don't use Math.Clamp since it throws
        // return new(Math.Clamp(point.x, x, right()), Math.Clamp(point.y, y, bottom()));

        float cx = MathF.Min(MathF.Max(point.X, X), Right);
        float cy = MathF.Min(MathF.Max(point.Y, Y), Bottom);
        return new PointF(cx, cy);
    }

    // Scales the rectangle by |scale|.
    public void Scale(float scale) => Scale(scale, scale);
    public void Scale(float x_scale, float y_scale)
    {
        m_Origin = PointF.ScalePoint(m_Origin, x_scale, y_scale);
        m_Size = SizeF.ScaleSize(m_Size, x_scale, y_scale);
    }

    // Divides the rectangle by |inv_scale|.
    public void InvScale(float inv_scale) => InvScale(inv_scale, inv_scale);
    public void InvScale(float inv_x_scale, float inv_y_scale)
    {
        m_Origin.InvScale(inv_x_scale, inv_y_scale);
        m_Size.InvScale(inv_x_scale, inv_y_scale);
    }

    // This method reports if the RectF can be safely converted to an integer Rect.
    // When it is false, some dimension of the RectF is outside the bounds of what an integer can represent,
    // and converting it to a Rect will require clamping.
    public readonly bool IsExpressibleAsRect()
    {
        static bool IsValueInRageForInt(float x) => x >= int.MinValue && x <= int.MaxValue;

        return  IsValueInRageForInt(X) &&
                IsValueInRageForInt(Y) &&
                IsValueInRageForInt(Width) &&
                IsValueInRageForInt(Height) &&
                IsValueInRageForInt(Right) &&
                IsValueInRageForInt(Bottom);
    }

    public readonly bool ApproximatelyEqual(in RectF rect, float tolerance_x, float tolerance_y) =>
        MathF.Abs(X - rect.X) <= tolerance_x &&
        MathF.Abs(Y - rect.Y) <= tolerance_y &&
        MathF.Abs(Right - rect.Right) <= tolerance_x &&
        MathF.Abs(Bottom - rect.Bottom) <= tolerance_y;

    public override readonly string ToString() => $"{m_Origin} {m_Size}";
    public override readonly int GetHashCode() => HashCode.Combine(m_Origin, m_Size);
    public override readonly bool Equals(object? obj) => obj is RectF other && Equals(other);
    public readonly bool Equals(in RectF other) => m_Origin.Equals(other.m_Origin) && m_Size.Equals(other.m_Size);

    private static void AdjustAlongAxis(float dst_origin, float dst_size, ref float origin, ref float size)
    {
        size = MathF.Min(dst_size, size);

        if (origin < dst_origin)
            origin = dst_origin;
        else
            origin = MathF.Min(dst_origin + dst_size, origin + size) - size;
    }

    public static bool operator ==(in RectF lhs, in RectF rhs) => lhs.Equals(rhs);
    public static bool operator !=(in RectF lhs, in RectF rhs) => !lhs.Equals(rhs);

    public static RectF operator +(in RectF lhs, in Vector2DF rhs) =>
        new RectF(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Width, lhs.Height);

    public static RectF operator -(in RectF lhs, in Vector2DF rhs) =>
        new RectF(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Width, lhs.Height);

    public static  RectF operator+(in Vector2DF lhs, in RectF rhs) => rhs + lhs;

    public static RectF IntersectRects(in RectF a, in RectF b)
    {
        RectF result = a;
        result.Intersect(b);
        return result;
    }

    public static RectF UnionRects(in RectF a, in RectF b)
    {
        RectF result = a;
        result.Union(b);
        return result;
    }

    // Note: Uses base::span<const RectF> in original C++ code, consider using C# Span types
    public static RectF UnionRects(IEnumerable<RectF> rects)
    {
        RectF result = new();
        foreach (var rect in rects)
            result.Union(rect);
        return result;
    }
    
    public static RectF UnionRectsEvenIfEmpty(in RectF a, in RectF b)
    {
        RectF result = a;
        result.UnionEvenIfEmpty(b);
        return result;
    }

    public static RectF SubtractRects(in RectF a, in RectF b)
    {
        RectF result = a;
        result.Subtract(b);
        return result;
    }

    public static RectF ScaleRect(in RectF r, float x_scale, float y_scale) => new RectF(r.X * x_scale, r.Y * y_scale, r.Width * x_scale, r.Height * y_scale);

    public static RectF ScaleRect(in RectF r, in SizeF size) => ScaleRect(r, size.Width, size.Height);

    public static RectF ScaleRect(in RectF r, in Size size) => ScaleRect(r, (SizeF)size);

    public static RectF ScaleRect(in RectF r, float scale) => ScaleRect(r, scale, scale);

    public static RectF TransposeRect(in RectF r) => new RectF(r.Y, r.X, r.Height, r.Width);

    // Construct a rectangle with top-left corner at |p1| and bottom-right corner
    // at |p2|. If the exact result of top - bottom or left - right cannot be
    // presented in float, then the height/width will be grown to the next
    // float, so that it includes both |p1| and |p2|.
    //
    // This could also be thought of as "the smallest rect that contains both
    // points", except that we consider points on the right/bottom edges of the
    // rect to be outside the rect.  So technically one or both points will not be
    // contained within the rect, because they will appear on one of these edges.
    public static RectF BoundingRect(in PointF p1, in PointF p2)
    {
        float left = MathF.Min(p1.X, p2.X);
        float top = MathF.Min(p1.Y, p2.Y);
        float right = MathF.Max(p1.X, p2.X);
        float bottom = MathF.Max(p1.Y, p2.Y);
        float width = right - left;
        float height = bottom - top;

        // If the precision is lost during the calculation, always grow to the next
        // value to include both ends.
        if (left + width != right)
        {
            width = MathF.BitIncrement(width);

            if (float.IsInfinity(width))
                width = float.MaxValue;
        }

        if (top + height != bottom)
        {
            height = MathF.BitIncrement(height);

            if (float.IsInfinity(height))
                height = float.MaxValue;
        }

        return new RectF(left, top, width, height);
    }

    // Return a maximum rectangle in which any point is covered by either a or b.
    public static RectF MaximumCoveredRect(in RectF a, in RectF b)
    {
        // Check a or b by itself.
        RectF maximum = a;
        float maximum_area = a.m_Size.GetArea();
        if (b.m_Size.GetArea() > maximum_area)
        {
            maximum = b;
            maximum_area = b.m_Size.GetArea();
        }

        // Check the regions that include the intersection of a and b. This can be
        // done by taking the intersection and expanding it vertically and
        // horizontally. These expanded intersections will both still be covered by
        // a or b.
        RectF intersection = a;
        intersection.InclusiveIntersect(b);
        if (!intersection.m_Size.IsZero)
        {
            RectF vert_expanded_intersection = intersection;
            vert_expanded_intersection.Y = MathF.Min(a.Y, b.Y);
            vert_expanded_intersection.Height = MathF.Max(a.Bottom, b.Bottom) - vert_expanded_intersection.Y;
            if (vert_expanded_intersection.m_Size.GetArea() > maximum_area)
            {
                maximum = vert_expanded_intersection;
                maximum_area = vert_expanded_intersection.m_Size.GetArea();
            }
            RectF horiz_expanded_intersection = intersection;
            horiz_expanded_intersection.X = MathF.Min(a.X, b.X);
            horiz_expanded_intersection.Width = MathF.Max(a.Right, b.Right) - horiz_expanded_intersection.X;
            if (horiz_expanded_intersection.m_Size.GetArea() > maximum_area)
            {
                maximum = horiz_expanded_intersection;
            }
        }
        return maximum;
    }

    // Returns the rect in |dest_rect| corresponding to |r] in |src_rect| when
    // |src_rect| is mapped to |dest_rect|.
    public static RectF MapRect(in RectF r, in RectF src_rect, in RectF dest_rect)
    {
        if (src_rect.IsEmpty())
            return new RectF();

        float width_scale = dest_rect.Width / src_rect.Width;
        float height_scale = dest_rect.Height / src_rect.Height;

        return new RectF(dest_rect.X + (r.X - src_rect.X) * width_scale,
            dest_rect.Y + (r.Y - src_rect.Y) * height_scale,
            r.Width * width_scale, r.Height * height_scale);
    }
}
