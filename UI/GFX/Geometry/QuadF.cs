using System.Diagnostics;
using System.Runtime.CompilerServices;
using UI.Extensions;

using static UI.GFX.Geometry.Triangle;

namespace UI.GFX.Geometry;

// A Quad is defined by four corners, allowing it to have edges that are not axis-aligned, unlike a Rect.
public struct QuadF
{
    private PointF p1_;
    private PointF p2_;
    private PointF p3_;
    private PointF p4_;

    public PointF p1 { readonly get => p1_; set => p1_ = value; }
    public PointF p2 { readonly get => p2_; set => p2_ = value; }
    public PointF p3 { readonly get => p3_; set => p3_ = value; }
    public PointF p4 { readonly get => p4_; set => p4_ = value; }
    
    public QuadF(in PointF p1, in PointF p2, in PointF p3, in PointF p4) => (p1_, p2_, p3_, p4_) = (p1, p2, p3, p4);

    public QuadF(in RectF rect)
    {
        p1_ = new(rect.X, rect.Y);
        p2_ = new(rect.Right, rect.Y);
        p3_ = new(rect.Right, rect.Bottom);
        p4_ = new(rect.X, rect.Bottom);
    }

    // Returns true if the quad is an axis-aligned rectangle.
    public readonly bool IsRectilinear()
    {
        static bool WithinEpsilon(float a, float b) => MathF.Abs(a - b) < float.MachineEpsilon;

        return
            (WithinEpsilon(p1_.X, p2_.X) && WithinEpsilon(p2_.Y, p3_.Y) &&
            WithinEpsilon(p3_.X, p4_.X) && WithinEpsilon(p4_.Y, p1_.Y)) ||
            (WithinEpsilon(p1_.Y, p2_.Y) && WithinEpsilon(p2_.X, p3_.X) &&
            WithinEpsilon(p3_.Y, p4_.Y) && WithinEpsilon(p4_.X, p1_.X));
    }

    // Returns true if the points of the quad are in counter-clockwise order. This
    // assumes that the quad is convex, and that no three points are collinear.
    public readonly bool IsCounterClockwise()
    {
        // This math computes the signed area of the quad. Positive area
        // indicates the quad is clockwise; negative area indicates the quad is
        // counter-clockwise. Note carefully: this is backwards from conventional
        // math because our geometric space uses screen coordiantes with y-axis
        // pointing downards.
        // Reference: http://mathworld.wolfram.com/PolygonArea.html.
        // The equation can be written:
        // Signed area = determinant1 + determinant2 + determinant3 + determinant4
        // In practise, Refactoring the computation of adding determinants so that
        // reducing the number of operations. The equation is:
        // Signed area = element1 + element2 - element3 - element4

        float p24 = p2_.Y - p4_.Y;
        float p31 = p3_.Y - p1_.Y;

        // Up-cast to double so this cannot overflow.
        double element1 = (double)p1_.X * p24;
        double element2 = (double)p2_.X * p31;
        double element3 = (double)p3_.X * p24;
        double element4 = (double)p4_.X * p31;

        return element1 + element2 < element3 + element4;
    }

    // Returns true if the |point| is contained within the quad, or lies on on
    // edge of the quad. This assumes that the quad is convex.
    public readonly bool Contains(in PointF point) =>
 
        PointIsInTriangle(point, p1_, p2_, p3_) ||
        PointIsInTriangle(point, p1_, p3_, p4_);

    // Returns true if the |quad| parameter is contained within |this| quad.
    // This method assumes |this| quad is convex. The |quad| parameter has no
    // restrictions.
    public readonly bool ContainsQuad(in QuadF other) =>
    
        Contains(other.p1) &&
        Contains(other.p2) &&
        Contains(other.p3) &&
        Contains(other.p4);

    // Returns two points (forming an axis-aligned bounding box)
    // that bounds the four points of the quad.
    readonly (PointF, PointF) Extents()
    {
        float rl = p1.X;
        float rr = p1.X;
        float rt = p1.Y;
        float rb = p1.Y;

        // Check p2
        rl = MathF.Min(rl, p2.X);
        rr = MathF.Max(rr, p2.X);
        rt = MathF.Min(rt, p2.Y);
        rb = MathF.Max(rb, p2.Y);

        // Check p3
        rl = MathF.Min(rl, p3.X);
        rr = MathF.Max(rr, p3.X);
        rt = MathF.Min(rt, p3.Y);
        rb = MathF.Max(rb, p3.Y);

        // Check p4
        rl = MathF.Min(rl, p4.X);
        rr = MathF.Max(rr, p4.X);
        rt = MathF.Min(rt, p4.Y);
        rb = MathF.Max(rb, p4.Y);

        return (new PointF(rl, rt), new PointF(rr, rb));
    }

    // Returns a rectangle that bounds the four points of the quad. The points of
    // the quad may lie on the right/bottom edge of the resulting rectangle,
    // rather than being strictly inside it.
    public readonly RectF BoundingBox()
    {
        (PointF min, PointF max) = Extents();

        return new RectF(min.X, min.Y, max.X - min.X, max.Y - min.Y);
    }

    // Realigns the corners in the quad by rotating them n corners to the right.
    public void Realign(int times)
    {
#if DEBUG
        Debug.Assert(times <= 4);
#endif
        for (int i = 0; i < times; ++i)
        {
            PointF temp = p1_;
            p1_ = p2_;
            p2_ = p3_;
            p3_ = p4_;
            p4_ = temp;
        }
    }

    // Scale each point in the quad by the |scale| factor.
    public void Scale(float scale) => Scale(scale, scale);

    // Scale each point in the quad by the scale factors along each axis.
    public void Scale(float x_scale, float y_scale)
    {
        p1_.Scale(x_scale, y_scale);
        p2_.Scale(x_scale, y_scale);
        p3_.Scale(x_scale, y_scale);
        p4_.Scale(x_scale, y_scale);
    }

    // Tests whether any part of the rectangle intersects with this quad.
    // This only works for convex quads.
    // This intersection is edge-inclusive and will return true even if the
    // intersecting area is empty (i.e., the intersection is a line or a point).
    public readonly bool IntersectsRect(in RectF rect)
    {
        // Start by checking this quad against the potential separating axes of the
        // rectangle. Since the rectangle is axis-aligned, we can just check for
        // intersection between the bounding boxes - if they don't intersect one of
        // the edges of the rectangle is a separating axis.
        (PointF min, PointF max) = Extents();

        if (min.Y > rect.Bottom || rect.Y > max.Y)
        {
            return false;
        }
        if (min.X > rect.Right || rect.X > max.X)
        {
        return false;
        }
        // None of the edges of the rectangle are a separating axis - test the edges of this quad.
        return IntersectsRectPartial(rect);
    }

    private static PointF RightMostCornerToVector(in RectF rect, in Vector2DF vector)
    {
        // Return the corner of the rectangle that if it is to the left of the vector
        // would mean all of the rectangle is to the left of the vector.
        // The vector here represents the side between two points in a clockwise
        // convex polygon.
        //
        //  Q  XXX
        // QQQ XXX   If the lower left corner of X is left of the vector that goes
        //  QQQ      from the top corner of Q to the right corner of Q, then all of X
        //   Q       is left of the vector, and intersection impossible.
        //
        PointF point;

        if (vector.X >= 0)
            point.Y = rect.Bottom;
        else
            point.Y = rect.Y;
        if (vector.Y >= 0)
            point.X = rect.X;
        else
            point.X = rect.Right;
        
        return point;
    }

    // Tests whether the line is contained by or intersected with the circle.
    private static bool LineIntersectsCircle(in PointF center, float radius, in PointF p0, in PointF p1)
    {
        float x0 = p0.X - center.X, y0 = p0.Y - center.Y;
        float x1 = p1.X - center.X, y1 = p1.Y - center.Y;
        float radius2 = radius * radius;
        if ((x0 * x0 + y0 * y0) <= radius2 || (x1 * x1 + y1 * y1) <= radius2)
            return true;
        if (p0 == p1)
            return false;

        float a = y0 - y1;
        float b = x1 - x0;
        float c = x0 * y1 - x1 * y0;
        float distance2 = c * c / (a * a + b * b);
        // If distance between the center point and the line > the radius,
        // the line doesn't cross (or is contained by) the ellipse.
        if (distance2 > radius2)
            return false;

        // The nearest point on the line is between p0 and p1?
        float x = -a * c / (a * a + b * b);
        float y = -b * c / (a * a + b * b);

        return (((x0 <= x && x <= x1) || (x0 >= x && x >= x1)) &&
                ((y0 <= y && y <= y1) || (y1 <= y && y <= y0)));
    }

    // Like the above, but only checks `rect` against the sides of quad ("does
    // half of the job"). Can be used if it is known beforehand that the bounding
    // box of the quad intersects `rect`.
    public readonly bool IntersectsRectPartial(in RectF rect)
    {
        // For each side of the quad clockwise we check if the rectangle is to the
        // left of it since only content on the right can overlap with the quad.
        // This only works if the quad is convex.
        Vector2DF v1, v2, v3, v4;

        // Ensure we use clockwise vectors.
        if (IsCounterClockwise())
        {
            v1 = p4_ - p1_;
            v2 = p1_ - p2_;
            v3 = p2_ - p3_;
            v4 = p3_ - p4_;
        }
        else
        {
            v1 = p2_ - p1_;
            v2 = p3_ - p2_;
            v3 = p4_ - p3_;
            v4 = p1_ - p4_;
        }

        PointF p = RightMostCornerToVector(rect, v1);
        if (Vector2DF.CrossProduct(v1, p - p1_) < 0)
            return false;

        p = RightMostCornerToVector(rect, v2);
        if (Vector2DF.CrossProduct(v2, p - p2_) < 0)
            return false;

        p = RightMostCornerToVector(rect, v3);
        if (Vector2DF.CrossProduct(v3, p - p3_) < 0)
            return false;

        p = RightMostCornerToVector(rect, v4);
        if (Vector2DF.CrossProduct(v4, p - p4_) < 0)
            return false;

        // If not all of the rectangle is outside one of the quad's four sides, then
        // that means at least a part of the rectangle is overlapping the quad.
        return true;
    }

    private readonly bool IsToTheLeftOfOrTouchingLine(in PointF p, in Vector2DF vector)
    {
        if (Vector2DF.CrossProduct(vector, p1_ - p) >= 0)
        {
            return false;
        }
        if (Vector2DF.CrossProduct(vector, p2_ - p) >= 0)
        {
            return false;
        }
        if (Vector2DF.CrossProduct(vector, p3_ - p) >= 0)
        {
            return false;
        }
        if (Vector2DF.CrossProduct(vector, p4_ - p) >= 0)
        {
            return false;
        }
        return true;
    }

    private readonly bool FullyOutsideOneEdge(in QuadF quad)
    {
        // For each side of the quad clockwise we check if the quad is to the left of
        // it since only content on the right can overlap with the quad. This only
        // works if the quads are convex.
        Vector2DF v1, v2, v3, v4;

        // Ensure we use clockwise vectors.
        if (IsCounterClockwise())
        {
            v1 = p4_ - p1_;
            v2 = p1_ - p2_;
            v3 = p2_ - p3_;
            v4 = p3_ - p4_;
        }
        else
        {
            v1 = p2_ - p1_;
            v2 = p3_ - p2_;
            v3 = p4_ - p3_;
            v4 = p1_ - p4_;
        }

        if (quad.IsToTheLeftOfOrTouchingLine(p1_, v1))
        {
            return true;
        }
        if (quad.IsToTheLeftOfOrTouchingLine(p2_, v2))
        {
            return true;
        }
        if (quad.IsToTheLeftOfOrTouchingLine(p3_, v3))
        {
            return true;
        }
        if (quad.IsToTheLeftOfOrTouchingLine(p4_, v4))
        {
            return true;
        }
        return false;
    }

    // Tests whether any part of the quad intersects with this quad.
    // This intersection is edge-inclusive.
    public readonly bool IntersectsQuad(in QuadF quad)
    {
        // Check if |quad| is fully outside one of the edges of this quad or vice
        // versa.
        return !FullyOutsideOneEdge(quad) && !quad.FullyOutsideOneEdge(this);
    }

    // Test whether any part of the circle/ellipse intersects with this quad.
    // Note that these two functions only work for convex quads.
    // These intersections are edge-inclusive and will return true even if the
    // intersecting area is empty (i.e., the intersection is a line or a point).
    public readonly bool IntersectsCircle(in PointF center, float radius)
    {
        return Contains(center) || LineIntersectsCircle(center, radius, p1_, p2_) ||
         LineIntersectsCircle(center, radius, p2_, p3_) ||
         LineIntersectsCircle(center, radius, p3_, p4_) ||
         LineIntersectsCircle(center, radius, p4_, p1_);
    }

    public readonly bool IntersectsEllipse(in PointF center, in SizeF radii)
    {
        // Transform the ellipse to an origin-centered circle whose radius is the
        // product of major radius and minor radius.  Here we apply the same
        // transformation to the quad.
        QuadF transformed_quad = this;
        transformed_quad -= center.OffsetFromOrigin();
        transformed_quad.Scale(radii.Height, radii.Width);

        PointF origin_point = new();
        return transformed_quad.IntersectsCircle(origin_point, radii.Height * radii.Width);
    }

    // The center of the quad. If the quad is the result of a affine-transformed
    // rectangle this is the same as the original center transformed.
    public readonly PointF CenterPoint()
    {
        return new PointF((p1_.X + p2_.X + p3_.X + p4_.X) / 4.0f,
                          (p1_.Y + p2_.Y + p3_.Y + p4_.Y) / 4.0f);
    }

    // Returns a string representation of quad.
    public override readonly string ToString() => $"{p1_};{p2_};{p3_};{p4_}";

    public override readonly int GetHashCode() => HashCode.Combine(p1_, p2_, p3_, p4_);

    public readonly bool Equals(in QuadF other) => p1_ == other.p1_ && p2_ == other.p2_ && p3_ == other.p3_ && p4_ == other.p4_;

    public override readonly bool Equals(object? obj) => obj is QuadF other && Equals(other);

    public static bool operator ==(in QuadF left, in QuadF right) => left.Equals(right);

    public static bool operator !=(in QuadF left, in QuadF right) => !left.Equals(right);

    public void operator +=(in Vector2DF rhs)
    {
        p1_ += rhs;
        p2_ += rhs;
        p3_ += rhs;
        p4_ += rhs;
    }

    public void operator -=(in Vector2DF rhs)
    {
        p1_ -= rhs;
        p2_ -= rhs;
        p3_ -= rhs;
        p4_ -= rhs;
    }

    // Add a vector to a quad, offseting each point in the quad by the vector.
    public static QuadF operator +(in QuadF lhs, in Vector2DF rhs)
    {
        QuadF result = lhs;
        result += rhs;
        return result;
    }

    // Subtract a vector from a quad, offseting each point in the quad by the
    // inverse of the vector.
    public static QuadF operator -(in QuadF lhs, in Vector2DF rhs)
    {
        QuadF result = lhs;
        result -= rhs;
        return result;
    }

    public static implicit operator QuadF(in RectF rect) => new QuadF(rect);
}
