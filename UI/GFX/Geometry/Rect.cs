using static UI.Numerics.ClampedMath;
using static UI.Numerics.SafeConversions;
using static UI.GFX.Geometry.RectConversions;

namespace UI.GFX.Geometry;

public struct Rect
{
    private Point m_Origin;
    private Size m_Size;

    public int X
    {
        readonly get => m_Origin.X;
        set
        {
            // Sets the X position while preserving the width.
            m_Origin.X = value;
            m_Size.Width = ClampWidthOrHeight(value, m_Size.Width);
        }
    }

    public int Y
    {
        readonly get => m_Origin.Y;
        set
        {
            // Sets the Y position while preserving the height.
            m_Origin.Y = value;
            m_Size.Height = ClampWidthOrHeight(value, m_Size.Height);
        }
    }

    public int Width
    {
        readonly get => m_Size.Width;
        set => m_Size.Width = ClampWidthOrHeight(m_Origin.X, value);
    }

    public int Height
    {
        readonly get => m_Size.Height;
        set => m_Size.Height = ClampWidthOrHeight(m_Origin.Y, value);
    }

    public Point Origin
    {
        readonly get => m_Origin;
        set
        {
            m_Origin = value;
            // Ensure that width and height remain valid.
            m_Size.Width = ClampWidthOrHeight(m_Origin.X, m_Size.Width);
            m_Size.Height = ClampWidthOrHeight(m_Origin.Y, m_Size.Height);
        }
    }

    public Size Size
    {
        readonly get => m_Size;
        set
        {
            m_Size.Width = ClampWidthOrHeight(m_Origin.X, value.Width);
            m_Size.Height = ClampWidthOrHeight(m_Origin.Y, value.Height);
        }
    }

    public readonly int Right => X + Width;
    public readonly int Bottom => Y + Height;

    public readonly Point TopRight => new(Right, Y);
    public readonly Point BottomLeft => new(X, Bottom);
    public readonly Point BottomRight => new(Right, Bottom);

    public readonly Point LeftCenter => new(X, Y + Height / 2);
    public readonly Point TopCenter => new(X + Width / 2, Y);
    public readonly Point RightCenter => new(Right, Y + Height / 2);
    public readonly Point BottomCenter => new(X + Width / 2, Bottom);

    public Rect(int width, int height)
    {
        m_Origin = new Point();
        m_Size = new Size(width, height);
    }

    public Rect(int x, int y, int width, int height)
    {
        m_Origin = new Point(x, y);
        m_Size = new Size(ClampWidthOrHeight(x, width), ClampWidthOrHeight(y, height));
    }

    public Rect(Size size)
    {
        m_Origin = new Point();
        m_Size = size;
    }

    public Rect(Point origin, Size size)
    {
        m_Origin = origin;
        m_Size = new Size(ClampWidthOrHeight(origin.X, size.Width), ClampWidthOrHeight(origin.Y, size.Height));
    }

    public readonly Vector2D OffsetFromOrigin() => new (X, Y);

    public void SetRect(int x, int y, int width, int height)
    {
        m_Origin.SetPoint(x, y);
        // Ensure that width and height remain valid.
        m_Size.Width = ClampWidthOrHeight(x, width);
        m_Size.Height = ClampWidthOrHeight(x, height);
    }

    // Use in place of SetRect() when you know the edges of the rectangle instead
    // of the dimensions, rather than trying to determine the width/height
    // yourself. This safely handles cases where the width/height would overflow.
    public void SetByBounds(int left, int top, int right, int bottom)
    {
        SetHorizontalBounds(left, right);
        SetVerticalBounds(top, bottom);
    }

    public void SetHorizontalBounds(int left, int right)
    {
        X = left;
        Width = ClampSub(right, left);
        if (this.Right != right)
        {
            AdjustForSaturatedRight(right);
        }
    }

    public void SetVerticalBounds(int top, int bottom)
    {
        Y = top;
        Height = ClampSub(bottom, top);
        if (this.Bottom != bottom)
        {
            AdjustForSaturatedBottom(bottom);
        }
    }

    // Shrink the rectangle by |inset| on all sides.
    public void Inset(int inset) => Inset(new Insets(inset));

    // Shrink the rectangle by the given |insets|
    public void Inset(in Insets insets)
    {
        m_Origin += new Vector2D(insets.Left, insets.Top);
        Width = ClampSub(Width, insets.Width);
        Height = ClampSub(Height, insets.Height);
    }

    // Expand the rectangle by |outset| on all sides.
    public void Outset(int outset) => Inset(-outset);

    // Expand the rectangle by the given |outsets|.
    public void Outset(in Outsets outsets) => Inset(outsets.ToInsets());

    // Move the rectangle by a horizontal and vertical distance.
    public void Offset(int horizontal, int vertical) => Offset(new Vector2D(horizontal, vertical));
    public void Offset(in Vector2D distance)
    {
        m_Origin += distance;
        // Ensure that width and height remain valid.
        m_Size.Width = ClampWidthOrHeight(X, Width);
        m_Size.Height = ClampWidthOrHeight(X, Height);
    }

    public static Rect operator +(Rect rect, in Vector2D vector)
    {
        rect.Offset(vector);
        return rect;
    }

    public static Rect operator -(Rect rect, in Vector2D vector)
    {
        rect.Offset(-vector);
        return rect;
    }

    public readonly Insets InsetsFrom(in Rect inner) => Insets.TLBR(inner.Y - Y, inner.X - X, Bottom - inner.Bottom, Right - inner.Right);

    // Returns true if the area of the rectangle is zero.
    public readonly bool IsEmpty() => m_Size.IsEmpty();

    // Returns true if the point identified by point_x and point_y falls inside
    // this rectangle.  The point (x, y) is inside the rectangle, but the
    // point (x + width, y + height) is not.
    public readonly bool Contains(int point_x, int point_y) => (point_x >= X) && (point_x < Right) && (point_y >= Y) && (point_y < Bottom);

    // Returns true if the specified point is contained by this rectangle.
    public readonly bool Contains(in Point point) => Contains(point.X, point.Y);

    // Returns true if this rectangle contains the specified rectangle.
    public readonly bool Contains(in Rect rect) => rect.X >= X && rect.Right <= Right && rect.Y >= Y && rect.Bottom <= Bottom;

    // Returns true if this rectangle intersects the specified rectangle.
    // An empty rectangle doesn't intersect any rectangle.
    public readonly bool Intersects(in Rect rect) => !(IsEmpty() || rect.IsEmpty() || rect.X >= Right || rect.Right <= X || rect.Y >= Bottom || rect.Bottom <= Y);

    // Sets this rect to be the intersection of this rectangle with the given rectangle.
    public void Intersect(in Rect rect)
    {
        if (IsEmpty() || rect.IsEmpty())
        {
            SetRect(0, 0, 0, 0); // Throws away empty position.
            return;
        }

        int left = Math.Max(X, rect.X);
        int top = Math.Max(Y, rect.Y);
        int new_right = Math.Min(Right, rect.Right);
        int new_bottom = Math.Min(Bottom, rect.Bottom);

        if (left >= new_right || top >= new_bottom)
        {
            SetRect(0, 0, 0, 0); // Throws away empty position.
            return;
        }

        SetByBounds(left, top, new_right, new_bottom);
    }

    // Sets this rect to be the intersection of itself and |rect| using
    // edge-inclusive geometry.  If the two rectangles overlap but the overlap
    // region is zero-area (either because one of the two rectangles is zero-area,
    // or because the rectangles overlap at an edge or a corner), the result is
    // the zero-area intersection.  The return value indicates whether the two
    // rectangle actually have an intersection, since checking the result for
    // isEmpty() is not conclusive.
    public bool InclusiveIntersect(in Rect rect)
    {
        int left = Math.Max(X, rect.X);
        int top = Math.Max(Y, rect.Y);
        int new_right = Math.Min(Right, rect.Right);
        int new_bottom = Math.Min(Bottom, rect.Bottom);

        // Return a clean empty rectangle for non-intersecting cases.
        if (left > new_right || top > new_bottom)
        {
            SetRect(0, 0, 0, 0);
            return false;
        }

        SetByBounds(left, top, new_right, new_bottom);
        return true;
    }

    // Sets this rect to be the union of this rectangle with the given rectangle.
    // The union is the smallest rectangle containing both rectangles if not
    // empty. If both rects are empty, this rect will become |rect|.
    public void Union(in Rect rect)
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
    public void UnionEvenIfEmpty(in Rect rect)
    {
        SetByBounds(Math.Min(X, rect.X), Math.Min(Y, rect.Y),
                    Math.Max(Right, rect.Right),
                    Math.Max(Bottom, rect.Bottom));
    }

    // Sets this rect to be the rectangle resulting from subtracting |rect| from
    // |*this|, i.e. the bounding rect of |Region(*this) - Region(rect)|.
    public void Subtract(in Rect rect)
    {
        if (!Intersects(rect))
            return;
        
        if (rect.Contains(this))
        {
            SetRect(0, 0, 0, 0);
            return;
        }

        int rx = X;
        int ry = Y;
        int rr = Right;
        int rb = Bottom;

        if (rect.Y <= Y && rect.Bottom >= Bottom)
        {
            // complete intersection in the y-direction
            if (rect.X <= X)
            {
                rx = rect.Right;
            }
            else if (rect.Right >= Right)
            {
                rr = rect.X;
            }
        }
        else if (rect.X <= X && rect.Right >= Right)
        {
            // complete intersection in the x-direction
            if (rect.Y <= Y)
            {
                ry = rect.Bottom;
            }
            else if (rect.Bottom >= Bottom)
            {
                rb = rect.Y;
            }
        }

        SetByBounds(rx, ry, rr, rb);
    }

    // Fits as much of the receiving rectangle into the supplied rectangle as
    // possible, becoming the result. For example, if the receiver had
    // a x-location of 2 and a width of 4, and the supplied rectangle had
    // an x-location of 0 with a width of 5, the returned rectangle would have
    // an x-location of 1 with a width of 4.
    public void AdjustToFit(in Rect rect)
    {
        int new_x = X;
        int new_y = Y;
        int new_width = Width;
        int new_height = Height;
        AdjustAlongAxis(rect.X, rect.Width, ref new_x, ref new_width);
        AdjustAlongAxis(rect.Y, rect.Height, ref new_y, ref new_height);
        SetRect(new_x, new_y, new_width, new_height);
    }

    // Returns the center of this rectangle.
    public readonly Point CenterPoint() => new Point(X + Width / 2, Y + Height / 2);

    // Becomes a rectangle that has the same center point but with a |size|.
    public void ToCenteredSize(in Size size)
    {
        int new_x = X + (Width - size.Width) / 2;
        int new_y = Y + (Height - size.Height) / 2;
        SetRect(new_x, new_y, size.Width, size.Height);
    }

    // Becomes a rectangle that has the same center point but with a size capped at given |size|.
    public void ClampToCenteredSize(in Size to_size)
    {
        Size new_size = Size;
        new_size.SetToMin(to_size);
        ToCenteredSize(new_size);
    }

    // Transpose x and y axis.
    public void Transpose() => SetRect(Y, X, Height, Width);

    // Splits `this` in two halves, `left_half` and `right_half`.
    public readonly void SplitVertically(out Rect left_half, out Rect right_half)
    {
        left_half = new Rect(X, Y, Width / 2, Height);
        right_half = new Rect(left_half.Right, Y, Width - left_half.Width, Height);
    }

    // Splits `this` in two halves, `top_half` and `bottom_half`.
    public readonly void SplitHorizontally(out Rect top_half, out Rect bottom_half)
    {
        top_half = new Rect(X, Y, Width, Height / 2);
        bottom_half = new Rect(X, top_half.Bottom, Width, Height - top_half.Height);
    }

    // Returns true if this rectangle shares an entire edge (i.e., same width or same height)
    // with the given rectangle, and the rectangles do not overlap.
    public readonly bool SharesEdgeWith(in Rect rect) => (Y == rect.Y && Height == rect.Height && (X == rect.Right || Right == rect.X)) || (X == rect.X && Width == rect.Width && (Y == rect.Bottom || Bottom == rect.Y));

    // Returns the manhattan distance from the rect to the point. If the point is inside the rect, returns 0.
    public readonly int ManhattanDistanceToPoint(in Point point)
    {
        int x_distance = Math.Max(0, Math.Max(X - point.X, point.X - Right));
        int y_distance = Math.Max(0, Math.Max(Y - point.Y, point.Y - Bottom));

        return x_distance + y_distance;
    }

    // Returns the manhattan distance between the contents of this rect and the
    // contents of the given rect. That is, if the intersection of the two rects
    // is non-empty then the function returns 0. If the rects share a side, it
    // returns the smallest non-zero value appropriate for int.
    public readonly int ManhattanInternalDistance(in Rect rect)
    {
        Rect c = this;
        c.Union(rect);

        int x = Math.Max(0, c.Width - Width - rect.Width + 1);
        int y = Math.Max(0, c.Height - Height - rect.Height + 1);
        
        return x + y;
    }

    public override readonly string ToString() => $"{m_Origin} {m_Size}";

    public readonly bool ApproximatelyEqual(in Rect rect, int tolerance) => Math.Abs(X - rect.X) <= tolerance && Math.Abs(Y - rect.Y) <= tolerance && Math.Abs(Right - rect.Right) <= tolerance && Math.Abs(Bottom - rect.Bottom) <= tolerance;

    public readonly bool Equals(in Rect other) => m_Origin.Equals(other.m_Origin) && m_Size.Equals(other.m_Size);
    public override readonly bool Equals(object? obj) => obj is Rect other && Equals(other);
    public override readonly int GetHashCode() => HashCode.Combine(m_Origin, m_Size);

    public static bool operator ==(in Rect left, in Rect right) => left.Equals(right);
    public static bool operator !=(in Rect left, in Rect right) => !left.Equals(right);

    // Clamp the width/height to avoid integer overflow in bottom() and right().
    // This returns the clamped width/height given an |x_or_y| and a
    // |width_or_height|.
    private static int ClampWidthOrHeight(int x_or_y, int width_or_height) => ClampAdd(x_or_y, width_or_height) - x_or_y;

    private void AdjustForSaturatedRight(int right)
    {
        SaturatedClampRange(X, right, out int new_x, out int width);
        X = new_x;
        m_Size.Width = width;
    }

    private void AdjustForSaturatedBottom(int bottom)
    {
        SaturatedClampRange(Y, bottom, out int new_y, out int height);
        Y = new_y;
        m_Size.Height = height;
    }
    
    private static void AdjustAlongAxis(int dst_origin, int dst_size, ref int origin, ref int size)
    {
      size = Math.Min(dst_size, size);

      if (origin < dst_origin)
        origin = dst_origin;
      else
        origin = Math.Min(dst_origin + dst_size, origin + size) - size;
    }

    // This is the per-axis heuristic for picking the most useful origin and width/height to represent the input range.
    private static void SaturatedClampRange(int min, int max, out int origin, out int span)
    {
        if (max < min)
        {
            span = 0;
            origin = min;
            return;
        }

        int effective_span = ClampSub(max, min);
        int span_loss = ClampSub(max, min + effective_span);

        // If the desired width is within the limits of ints,
        // we can just use the simple computations to represent the range precisely.
        if (span_loss == 0)
        {
            span = effective_span;
            origin = min;
            return;
        }

        // Now we have to approximate. If one of min or max is close enough
        // to zero we choose to represent that one precisely.
        // The other side is probably practically "infinite", so we move it.
        const uint MaxDimension = int.MaxValue / 2;
        if (SafeUnsignedAbs(max) < MaxDimension)
        {
            // Maintain origin + span == max.
            span = effective_span;
            origin = max - effective_span;
        }
        else if (SafeUnsignedAbs(min) < MaxDimension)
        {
            // Maintain origin == min.
            span = effective_span;
            origin = min;
        }
        else
        {
            // Both are big, so keep the center.
            span = effective_span;
            origin = min + span_loss / 2;
        }
    }

    public static Rect IntersectRects(in Rect a, in Rect b)
    {
        Rect result = a;
        result.Intersect(b);
        return result;
    }

    public static Rect UnionRects(in Rect a, in Rect b)
    {
        Rect result = a;
        result.Union(b);
        return result;
    }

    // Note: This originally uses base::span<const Rect> in C++, consider using a C# Span type
    public static Rect UnionRects(IEnumerable<Rect> rects)
    {
        Rect result = new ();
        foreach (var rect in rects)
        {
            result.Union(rect);
        }
        return result;
    }

    public static Rect UnionRectsEvenIfEmpty(in Rect a, in Rect b)
    {
        Rect result = a;
        result.UnionEvenIfEmpty(b);
        return result;
    }

    public static Rect SubtractRects(in Rect a, in Rect b)
    {
        Rect result = a;
        result.Subtract(b);
        return result;
    }

    // Constructs a rectangle with |p1| and |p2| as opposite corners.
    //
    // This could also be thought of as "the smallest rect that contains both
    // points", except that we consider points on the right/bottom edges of the
    // rect to be outside the rect.  So technically one or both points will not be
    // contained within the rect, because they will appear on one of these edges.
    public static Rect BoundingRect(in Point p1, in Point p2)
    {
        Rect result = new();

        result.SetByBounds(Math.Min(p1.X, p2.X),
                           Math.Min(p1.Y, p2.Y),
                           Math.Max(p1.X, p2.X),
                           Math.Max(p1.Y, p2.Y));

        return result;
    }

    // Scales the rect and returns the enclosing rect. The components are clamped if they would overflow.
    public static Rect ScaleToEnclosingRect(in Rect rect, float x_scale, float y_scale)
    {
        if (x_scale == 1f && y_scale == 1f)
            return rect;
        int x = ClampFloor(rect.X * x_scale);
        int y = ClampFloor(rect.Y * y_scale);
        int r = rect.Width == 0 ? x : ClampCeil(rect.Right * x_scale);
        int b = rect.Height == 0 ? y : ClampCeil(rect.Bottom * y_scale);
        Rect result = new();
        result.SetByBounds(x, y, r, b);
        return result;
    }
    
    public static Rect ScaleToEnclosingRect(in Rect rect, float scale) => ScaleToEnclosingRect(rect, scale, scale);
    
    public static Rect ScaleToEnclosedRect(in Rect rect, float x_scale, float y_scale)
    {
        if (x_scale == 1f && y_scale == 1f)
            return rect;
        int x = ClampCeil(rect.X * x_scale);
        int y = ClampCeil(rect.Y * y_scale);
        int r = rect.Width == 0 ? x : ClampFloor(rect.Right * x_scale);
        int b = rect.Height == 0 ? y : ClampFloor(rect.Bottom * y_scale);
        Rect result = new();
        result.SetByBounds(x, y, r, b);
        return result;
    }
    
    public static Rect ScaleToEnclosedRect(in Rect rect, float scale) => ScaleToEnclosedRect(rect, scale, scale);

    // Scales |rect| by scaling its four corner points. If the corner points lie on
    // non-integral coordinate after scaling, their values are rounded to the
    // nearest integer. The components are clamped if they would overflow.
    // This is helpful during layout when relative positions of multiple gfx::Rect
    // in a given coordinate space needs to be same after scaling as it was before
    // scaling. ie. this gives a lossless relative positioning of rects.
    public static Rect ScaleToRoundedRect(in Rect rect, float x_scale, float y_scale)
    {
        if (x_scale == 1f && y_scale == 1f)
            return rect;
        int x = ClampRound(rect.X * x_scale);
        int y = ClampRound(rect.Y * y_scale);
        int r = rect.Width == 0 ? x : ClampRound(rect.Right * x_scale);
        int b = rect.Height == 0 ? y : ClampRound(rect.Bottom * y_scale);
        Rect result = new Rect();
        result.SetByBounds(x, y, r, b);
        return result;
    }
    
    public static Rect ScaleToRoundedRect(in Rect rect, float scale) => ScaleToRoundedRect(rect, scale, scale);

    // Scales `rect` by `scale` and rounds to enclosing rect, but for each edge, if
    // the distance between the edge and the nearest integer grid is smaller than
    // `error`, the edge is snapped to the integer grid.  The default error is 0.001
    // , which is used by cc/viz. Use this when scaling the window/layer size.
    public static Rect ScaleToEnclosingRectIgnoringError(in Rect rect, float scale, float epsilon = 0.001f)
    {
        RectF rect_f = new (rect);
        rect_f.Scale(scale);
        return ToEnclosingRectIgnoringError(rect_f, epsilon);
    }

    // Return a maximum rectangle that is covered by the a or b.
    public static Rect MaximumCoveredRect(in Rect a, in Rect b)
    {
        // Check a or b by itself.
        Rect maximum = a;
        ulong maximum_area = (ulong)a.Size.Area64();
        if ((ulong)b.Size.Area64() > maximum_area)
        {
            maximum = b;
            maximum_area = (ulong) b.Size.Area64();
        }
        // Check the regions that include the intersection of a and b. This can be
        // done by taking the intersection and expanding it vertically and
        // horizontally. These expanded intersections will both still be covered by
        // a or b.
        Rect intersection = a;
        intersection.InclusiveIntersect(b);

        if (!intersection.Size.IsZero())
        {
            Rect vert_expanded_intersection = intersection;
            vert_expanded_intersection.SetVerticalBounds(Math.Min(a.Y, b.Y), Math.Max(a.Bottom, b.Bottom));

            if ((ulong)vert_expanded_intersection.Size.Area64() > maximum_area)
            {
                maximum = vert_expanded_intersection;
                maximum_area = (ulong) vert_expanded_intersection.Size.Area64();
            }

            Rect horiz_expanded_intersection = intersection;

            horiz_expanded_intersection.SetHorizontalBounds(Math.Min(a.X, b.X), Math.Max(a.Right, b.Right));
            
            if ((ulong)horiz_expanded_intersection.Size.Area64() > maximum_area)
            {
                maximum = horiz_expanded_intersection;

                // This line is unnecessary since maximum_area is a local variable and it won't be used further
                //maximum_area = (ulong)horiz_expanded_intersection.size.Area64();
            }
        }

        return maximum;
    }
}
