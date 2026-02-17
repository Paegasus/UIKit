using System.Diagnostics;
using UI.Extensions;

using static UI.Numerics.SafeConversions;

namespace UI.GFX.Geometry;

// A floating version of Rect.
public struct RectF : IEquatable<RectF>
{
    private PointF origin_;
    private SizeF size_;

    public RectF(float width, float height) : this(0, 0, width, height) { }
    public RectF(float x, float y, float width, float height) 
    {
        origin_ = new PointF(x, y);
        size_ = new SizeF(width, height);
    }
    public RectF(in SizeF size) : this(0, 0, size.width, size.height) { }
    public RectF(in PointF origin, in SizeF size) : this(origin.x, origin.y, size.width, size.height) { }
    public RectF(in Rect r) : this(r.x(), r.y(), r.width(), r.height()) { }

    public float x { readonly get => origin_.x; set => origin_.x = value; }
    public float y { readonly get => origin_.y; set => origin_.y = value; }
    public float width { readonly get => size_.width; set => size_.width = value; }
    public float height { readonly get => size_.height; set => size_.height = value; }

    public PointF origin { readonly get => origin_; set => origin_ = value; }
    public SizeF size { readonly get => size_; set => size_ = value; }

    public readonly float right() => x + width;
    public readonly float bottom() => y + height;

    public readonly PointF top_right() => new(right(), y);
    public readonly PointF bottom_left() => new(x, bottom());
    public readonly PointF bottom_right() => new(right(), bottom());

    public readonly PointF left_center() => new(x, y + height / 2);
    public readonly PointF top_center() => new(x + width / 2, y);
    public readonly PointF right_center() => new(right(), y + height / 2);
    public readonly PointF bottom_center() => new(x + width / 2, bottom());

    public readonly Vector2DF OffsetFromOrigin() => new(x, y);

    public void SetRect(float x, float y, float width, float height)
    {
        origin_.SetPoint(x, y);
        size_.SetSize(width, height);
    }

    // Shrinks the rectangle by |inset| on all sides.
    public void Inset(float inset) => Inset(new InsetsF(inset));

    // Shrinks the rectangle by the given |insets|.
    public void Inset(in InsetsF insets)
    {
        origin_.Offset(insets.left(), insets.top());
        width -= insets.width();
        height -= insets.height();
    }

    // Expands the rectangle by |outset| on all sides.
    public void Outset(float outset) => Inset(-outset);

    // Expands the rectangle by the given |outsets|.
    public void Outset(in OutsetsF outsets) => Inset(outsets.ToInsets());

    // Move the rectangle by a horizontal and vertical distance.
    public void Offset(float horizontal, float vertical) => origin_.Offset(horizontal, vertical);
    public void Offset(in Vector2DF distance) => origin_.Offset(distance.x, distance.y);

    public readonly InsetsF InsetsFrom(in RectF inner) =>
        InsetsF.TLBR(inner.y - y, inner.x - x, bottom() - inner.bottom(), right() - inner.right());

    // Returns true if the area of the rectangle is zero.
    public readonly bool IsEmpty() => size_.IsEmpty();

    // Returns true if the point identified by point_x and point_y falls inside
    // this rectangle (including the left and the top edges, excluding the right
    // and the bottom edges). If this rectangle is empty, this method returns
    // false regardless of the point.
    public readonly bool Contains(float point_x, float point_y) =>
        point_x >= x && point_x < right() && point_y >= y && point_y < bottom();

    // Returns true if the specified point is contained by this rectangle.
    public readonly bool Contains(in PointF point) => Contains(point.x, point.y);
    
    // Returns true if this rectangle contains the specified rectangle.
    public readonly bool Contains(in RectF rect) =>
        rect.x >= x && rect.right() <= right() && rect.y >= y && rect.bottom() <= bottom();
    
    // Similar to Contains(), but uses edge-inclusive geometry, i.e. also returns
    // true if the point is on the right or the bottom edge. If this rectangle
    // is empty, this method returns true only if the point is at the origin of
    // this rectangle.
    public readonly bool InclusiveContains(float point_x, float point_y) =>
        point_x >= x && point_x <= right() && point_y >= y && point_y <= bottom();

    public readonly bool InclusiveContains(in PointF point) => InclusiveContains(point.x, point.y);

    // Returns true if this rectangle intersects the specified rectangle.
    // An empty rectangle doesn't intersect any rectangle.
    public readonly bool Intersects(in RectF rect) =>
        !IsEmpty() && !rect.IsEmpty() && rect.x < right() && rect.right() > x && rect.y < bottom() &&
        rect.bottom() > y;

    // Sets this rect to be the intersection of this rectangle with the given rectangle.
    public void Intersect(in RectF rect)
    {
        if (IsEmpty() || rect.IsEmpty())
        {
            SetRect(0, 0, 0, 0);
            return;
        }

        float rx = Math.Max(x, rect.x);
        float ry = Math.Max(y, rect.y);
        float rr = Math.Min(right(), rect.right());
        float rb = Math.Min(bottom(), rect.bottom());
        
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
        float rx = Math.Max(x, rect.x);
        float ry = Math.Max(y, rect.y);
        float rr = Math.Min(right(), rect.right());
        float rb = Math.Min(bottom(), rect.bottom());

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
        float rx = Math.Min(x, rect.x);
        float ry = Math.Min(y, rect.y);
        float rr = Math.Max(right(), rect.right());
        float rb = Math.Max(bottom(), rect.bottom());
        
        SetRect(rx, ry, rr - rx, rb - ry);

        // Due to floating errors and SizeF::clamp(), the new rect may not fully
        // contain the original rects at the right/bottom side. Expand the rect in the case.

        if (right() < rr && width < float.MaxValue)
        
            size_.SetToNextWidth();
#if DEBUG
            //DCHECK_GE(right(), rr);
            Debug.Assert(right() >= rr, "RectF.UnionEvenIfEmpty(): right() should be >= rr.");
#endif
        
        if (bottom() < rb && height < float.MaxValue)
        {
            size_.SetToNextHeight();
#if DEBUG
            //DCHECK_GE(bottom(), rb);
            Debug.Assert(bottom() >= rb, "RectF.UnionEvenIfEmpty(): bottom() should be >= rb.");
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

        float rx = x;
        float ry = y;
        float rr = right();
        float rb = bottom();

        if (rect.y <= y && rect.bottom() >= bottom())
        {
            // complete intersection in the y-direction

            if (rect.x <= x)
                rx = rect.right();

            else if (rect.right() >= right())
                rr = rect.x;
        }
        else if (rect.x <= x && rect.right() >= right())
        {
            // complete intersection in the x-direction

            if (rect.y <= y)
                ry = rect.bottom();

            else if (rect.bottom() >= bottom())
                rb = rect.y;
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
        float new_x = x;
        float new_y = y;
        float new_width = width;
        float new_height = height;
        AdjustAlongAxis(rect.x, rect.width, ref new_x, ref new_width);
        AdjustAlongAxis(rect.y, rect.height, ref new_y, ref new_height);
        SetRect(new_x, new_y, new_width, new_height);
    }

    // Returns the center of this rectangle.
    public readonly PointF CenterPoint() => new(x + width / 2, y + height / 2);

    // Becomes a rectangle that has the same center point but with a size capped at given |size|.
    public void ClampToCenteredSize(in SizeF size)
    {
        float new_width = Math.Min(width, size.width);
        float new_height = Math.Min(height, size.height);
        float new_x = x + (width - new_width) / 2;
        float new_y = y + (height - new_height) / 2;
        SetRect(new_x, new_y, new_width, new_height);
    }

    // Transpose x and y axis.
    public void Transpose() => SetRect(y, x, height, width);

    // Splits `this` in two halves, `left_half` and `right_half`.
    public void SplitVertically(out RectF left_half, out RectF right_half)
    {
        left_half = new RectF(x, y, width / 2, height);
        right_half = new RectF(left_half.right(), y, width - left_half.width, height);
    }

    // Splits `this` in two halves, `top_half` and `bottom_half`.
    public void SplitHorizontally(out RectF top_half, out RectF bottom_half)
    {
        top_half = new RectF(x, y, width, height / 2);
        bottom_half = new RectF(x, top_half.bottom(), width, height - top_half.height);
    }

    // Returns true if this rectangle shares an entire edge (i.e., same width or
    // same height) with the given rectangle, and the rectangles do not overlap.
    public readonly bool SharesEdgeWith(in RectF rect) =>
        (y == rect.y && height == rect.height && (x == rect.right() || right() == rect.x)) ||
        (x == rect.x && width == rect.width && (y == rect.bottom() || bottom() == rect.y));

    // Returns the manhattan distance from the rect to the point. If the point is
    // inside the rect, returns 0.
    public readonly float ManhattanDistanceToPoint(in PointF point) =>
        Math.Max(0, Math.Max(x - point.x, point.x - right())) +
        Math.Max(0, Math.Max(y - point.y, point.y - bottom()));

    // Returns the manhattan distance between the contents of this rect and the
    // contents of the given rect. That is, if the intersection of the two rects
    // is non-empty then the function returns 0. If the rects share a side, it
    // returns the smallest non-zero value appropriate for float.
    public float ManhattanInternalDistance(in RectF rect)
    {
        RectF c = this;
        c.Union(rect);
        
        float x = Math.Max(0, c.width - width - rect.width + float.MachineEpsilon);
        float y = Math.Max(0, c.height - height - rect.height + float.MachineEpsilon);
        return x + y;
    }

    // Returns the closest point in or on an edge of this rect to the given point.
    public readonly PointF ClosestPoint(in PointF point)
    {
        // Don't use Math.Clamp since it throws
        // return new(Math.Clamp(point.x, x, right()), Math.Clamp(point.y, y, bottom()));

        float cx = MathF.Min(MathF.Max(point.x, x), right());
        float cy = MathF.Min(MathF.Max(point.y, y), bottom());
        return new PointF(cx, cy);
    }

    // Scales the rectangle by |scale|.
    public void Scale(float scale) => Scale(scale, scale);
    public void Scale(float x_scale, float y_scale)
    {
        origin = PointF.ScalePoint(origin, x_scale, y_scale);
        size = SizeF.ScaleSize(size, x_scale, y_scale);
    }

    // Divides the rectangle by |inv_scale|.
    public void InvScale(float inv_scale) => InvScale(inv_scale, inv_scale);
    public void InvScale(float inv_x_scale, float inv_y_scale)
    {
        origin.InvScale(inv_x_scale, inv_y_scale);
        size.InvScale(inv_x_scale, inv_y_scale);
    }

    // This method reports if the RectF can be safely converted to an integer Rect.
    // When it is false, some dimension of the RectF is outside the bounds of what an integer can represent,
    // and converting it to a Rect will require clamping.
    public readonly bool IsExpressibleAsRect()
    {
        static bool IsValueInRageForInt(float x) => x >= int.MinValue && x <= int.MaxValue;

        return  IsValueInRageForInt(x) &&
                IsValueInRageForInt(y) &&
                IsValueInRageForInt(width) &&
                IsValueInRageForInt(height) &&
                IsValueInRageForInt(right()) &&
                IsValueInRageForInt(bottom());
    }

    public readonly bool ApproximatelyEqual(in RectF rect, float tolerance_x, float tolerance_y) =>
        MathF.Abs(x - rect.x) <= tolerance_x &&
        MathF.Abs(y - rect.y) <= tolerance_y &&
        MathF.Abs(right() - rect.right()) <= tolerance_x &&
        MathF.Abs(bottom() - rect.bottom()) <= tolerance_y;

    public override readonly string ToString() => $"{origin_} {size_}";
    public override readonly int GetHashCode() => HashCode.Combine(origin_, size_);
    public override bool Equals(object? obj) => obj is RectF other && Equals(other);
    public bool Equals(RectF other) => origin_.Equals(other.origin_) && size_.Equals(other.size_);

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
        new RectF(lhs.x + rhs.x, lhs.y + rhs.y, lhs.width, lhs.height);

    public static RectF operator -(in RectF lhs, in Vector2DF rhs) =>
        new RectF(lhs.x - rhs.x, lhs.y - rhs.y, lhs.width, lhs.height);

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
        RectF result = new RectF();
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

    public static RectF ScaleRect(in RectF r, float x_scale, float y_scale) => new RectF(r.x * x_scale, r.y * y_scale, r.width * x_scale, r.height * y_scale);

    public static RectF ScaleRect(in RectF r, in SizeF size) => ScaleRect(r, size.width, size.height);

    public static RectF ScaleRect(in RectF r, in Size size) => ScaleRect(r, (SizeF)size);

    public static RectF ScaleRect(in RectF r, float scale) => ScaleRect(r, scale, scale);

    public static RectF TransposeRect(in RectF r) => new RectF(r.y, r.x, r.height, r.width);

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
        float left = MathF.Min(p1.x, p2.x);
        float top = MathF.Min(p1.y, p2.y);
        float right = MathF.Max(p1.x, p2.x);
        float bottom = MathF.Max(p1.y, p2.y);
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
        float maximum_area = a.size.GetArea();
        if (b.size.GetArea() > maximum_area)
        {
            maximum = b;
            maximum_area = b.size.GetArea();
        }

        // Check the regions that include the intersection of a and b. This can be
        // done by taking the intersection and expanding it vertically and
        // horizontally. These expanded intersections will both still be covered by
        // a or b.
        RectF intersection = a;
        intersection.InclusiveIntersect(b);
        if (!intersection.size.IsZero())
        {
            RectF vert_expanded_intersection = intersection;
            vert_expanded_intersection.y = MathF.Min(a.y, b.y);
            vert_expanded_intersection.height = MathF.Max(a.bottom(), b.bottom()) - vert_expanded_intersection.y;
            if (vert_expanded_intersection.size.GetArea() > maximum_area)
            {
                maximum = vert_expanded_intersection;
                maximum_area = vert_expanded_intersection.size.GetArea();
            }
            RectF horiz_expanded_intersection = intersection;
            horiz_expanded_intersection.x = MathF.Min(a.x, b.x);
            horiz_expanded_intersection.width = MathF.Max(a.right(), b.right()) - horiz_expanded_intersection.x;
            if (horiz_expanded_intersection.size.GetArea() > maximum_area)
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

        float width_scale = dest_rect.width / src_rect.width;
        float height_scale = dest_rect.height / src_rect.height;
        return new RectF(dest_rect.x + (r.x - src_rect.x) * width_scale,
            dest_rect.y + (r.y - src_rect.y) * height_scale,
            r.width * width_scale, r.height * height_scale);
    }
}
