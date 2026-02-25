using System.Diagnostics;
using SkiaSharp;

using static UI.GFX.Geometry.SkiaConversions;

namespace UI.GFX.Geometry;

public struct RRectF
{
    // These are all mutually exclusive, and ordered in increasing complexity. The
    // order is assumed in several functions.
    public enum RoundRectType
    {
        kEmpty,   // Zero width or height.
        kRect,    // Non-zero width and height, and zeroed radii - a pure rectangle.
        kSingle,  // Non-zero width and height, and a single, non-zero value for all
                  // X and Y radii.
        kSimple,  // Non-zero width and height, X radii all equal and non-zero, Y
                  // radii all equal and non-zero, and x_rad != y_rad.
        kOval,    // Non-zero width and height, X radii all equal to width/2, and Y
                  // radii all equal to height/2, and x_rad != y_rad.
        kComplex,  // Non-zero width and height, and arbitrary (non-equal) radii.
    }

    // Enumeration of the corners of a rectangle in clockwise order. Values match
    // SkRRect::Corner.
    public enum RoundRectCorner
    {
        kUpperLeft = SKRoundRectCorner.UpperLeft,
        kUpperRight = SKRoundRectCorner.UpperRight,
        kLowerRight = SKRoundRectCorner.LowerRight,
        kLowerLeft = SKRoundRectCorner.LowerLeft
    }

    private SKRoundRect skrrect_;

    public RRectF() => skrrect_ = new();

    public RRectF(in SKRoundRect rect) => skrrect_ = rect;

    public RRectF(in RectF rect) : this(rect, 0.0f) {}

    public RRectF(in RectF rect, float radius) : this(rect, radius, radius) {}
    public RRectF(in RectF rect, float x_rad, float y_rad) : this(rect.X, rect.Y, rect.Width, rect.Height, x_rad, y_rad) {}
    
    // Sets all x and y radii to radius.
    public RRectF(float x, float y, float width, float height, float radius) : this(x, y, width, height, radius, radius) {}
    
    // Sets all x radii to x_rad, and all y radii to y_rad.
    // If one of x_rad or y_rad are zero, sets ALL radii to zero.
    public RRectF(float x, float y, float width, float height, float x_rad, float y_rad)
    {
        // This is wrong in SkiaSharp
        //skrrect_ = new SKRoundRect(new SKRect(x, y, width, height), x_rad, y_rad);

        // SKRect() takes left, top, right, bottom (not left, top, width, height, like in C++ Skia), so:
        // right = x + width
        // bottom = y + height
        skrrect_ = new SKRoundRect(new SKRect(x, y, x + width, y + height), x_rad, y_rad);

        if (IsEmpty())
        {
            // Make sure that empty rects are created fully empty, not with some
            // non-zero dimensions.
            skrrect_ = new SKRoundRect();
        }
    }

    // Directly sets all four corners.
    public RRectF(float x,
         float y,
         float width,
         float height,
         float upper_left_x,
         float upper_left_y,
         float upper_right_x,
         float upper_right_y,
         float lower_right_x,
         float lower_right_y,
         float lower_left_x,
         float lower_left_y)
    {
        
    }

    public RRectF(in RectF rect,
                float upper_left_x,
                float upper_left_y,
                float upper_right_x,
                float upper_right_y,
                float lower_right_x,
                float lower_right_y,
                float lower_left_x,
                float lower_left_y) : this(rect.X,
                                           rect.Y,
                                           rect.Width,
                                           rect.Height,
                                           upper_left_x,
                                           upper_left_y,
                                           upper_right_x,
                                           upper_right_y,
                                           lower_right_x,
                                           lower_right_y,
                                           lower_left_x,
                                           lower_left_y) { }
    RRectF(in RectF rect, in RoundedCornersF corners) : this(rect.X,
                                                             rect.Y,
                                                             rect.Width,
                                                             rect.Height,
                                                             corners.UpperLeft,
                                                             corners.UpperLeft,
                                                             corners.UpperRight,
                                                             corners.UpperRight,
                                                             corners.LowerRight,
                                                             corners.LowerRight,
                                                             corners.LowerLeft,
                                                             corners.LowerLeft) { }
    public readonly RoundRectType GetRoundRectType()
    {
        SKPoint rad;

        switch (skrrect_.Type)
        {
            case SKRoundRectType.Empty:
                return RoundRectType.kEmpty;
            case SKRoundRectType.Rect:
                return RoundRectType.kRect;
            case SKRoundRectType.Simple:
                rad = skrrect_.GetRadii(SKRoundRectCorner.UpperLeft);
                if (rad.X == rad.Y)
                {
                    return RoundRectType.kSingle;
                }
                return RoundRectType.kSimple;
            case SKRoundRectType.Oval:
                rad = skrrect_.GetRadii(SKRoundRectCorner.UpperLeft);
                if (rad.X == rad.Y)
                {
                    return RoundRectType.kSingle;
                }
                return RoundRectType.kOval;
            case SKRoundRectType.NinePatch:
            case SKRoundRectType.Complex:
            default:
                return RoundRectType.kComplex;
        }
    }

    // The rectangular portion of the RRectF, without the corner radii.
    public readonly RectF rect()
    {
        return SkRectToRectF(skrrect_.Rect);
    }

    // Returns the radii of the all corners. DCHECKs that all corners
    // have the same radii (the type is <= kOval).
    public readonly Vector2DF GetSimpleRadii()
    {
#if DEBUG
        Debug.Assert(GetRoundRectType() <= RoundRectType.kOval);
#endif
        SKPoint result = skrrect_.GetRadii(SKRoundRectCorner.UpperLeft);
        return new Vector2DF(result.X, result.Y);
    }

    // Returns the radius of all corners. DCHECKs that all corners have the same
    // radii, and that x_rad == y_rad (the type is <= kSingle).
    public readonly float GetSimpleRadius()
    {
#if DEBUG
        Debug.Assert(GetRoundRectType() <= RoundRectType.kOval);
#endif
        SKPoint result = skrrect_.GetRadii(SKRoundRectCorner.UpperLeft);
#if DEBUG
        Debug.Assert(result.X == result.Y);
#endif
        return result.X;
    }

    // Make the RRectF empty.
    public void Clear()
    {
        skrrect_.SetEmpty();
    }

    public readonly bool IsEmpty()
    {
        return GetRoundRectType() == RoundRectType.kEmpty;
    }

    public readonly bool HasRoundedCorners()
    {
        return !IsEmpty() && GetRoundRectType() != RoundRectType.kRect;
    }

    // GetCornerRadii may be called for any type of RRect (kRect, kOval, etc.),
    // and it will return "correct" values. If GetType() is kOval or less,
    // all corner values will be identical to each other. SetCornerRadii can similarly
    // be called on any type of RRect, but GetType() may change as a result of the call.
    public readonly Vector2DF GetCornerRadii(RoundRectCorner corner)
    {
        SKPoint result = skrrect_.GetRadii((SKRoundRectCorner)corner);
        
        return new Vector2DF(result.X, result.Y);
    }

    private readonly Span<SKPoint> GetAllRadii()
    {
        return skrrect_.Radii.AsSpan();
    }

    void SetCornerRadii(RoundRectCorner corner, float x_rad, float y_rad)
    {
        var radii = GetAllRadii();
        
        radii[(int)corner] = new SKPoint(x_rad, y_rad);

        skrrect_.SetRectRadii(skrrect_.Rect, radii);
    }

    void SetCornerRadii(RoundRectCorner corner, in Vector2DF radii)
    {
        SetCornerRadii(corner, radii.X, radii.Y);
    }

    // Returns true if |rect| is inside the bounds and corner radii of this
    // RRectF, and if both this RRectF and rect are not empty.
    public readonly bool Contains(in RectF rect)
    {
        return skrrect_.Contains(RectFToSkRect(rect));
    }

    // Move the rectangle by a horizontal and vertical distance.
    public void Offset(float horizontal, float vertical)
    {
        skrrect_.Offset(horizontal, vertical);
    }

    public void Offset(in Vector2DF distance)
    {
        Offset(distance.X, distance.Y);
    }

    public override readonly int GetHashCode() => HashCode.Combine(skrrect_.Radii[0], skrrect_.Radii[1], skrrect_.Radii[2], skrrect_.Radii[3],
                                                                   skrrect_.Rect.Top, skrrect_.Rect.Right, skrrect_.Rect.Bottom, skrrect_.Rect.Right);
    public override readonly bool Equals(object? obj) => obj is RRectF other && Equals(other);

    public readonly bool Equals(in RRectF other)
    {
        // Skia normalizes on construction, so two empty rrects are always equal
        // regardless of their original bounds or radii (e.g. RRectF(1,2,3,0,5,6)
        // normalizes to the same Empty type as RRectF(0,0,0,0,0,0)).
        if (skrrect_.Type != other.skrrect_.Type)
            return false;

        if (skrrect_.Type == SKRoundRectType.Empty)
            return true;

        return skrrect_.Rect == other.skrrect_.Rect &&
               skrrect_.GetRadii(SKRoundRectCorner.UpperLeft) == other.skrrect_.GetRadii(SKRoundRectCorner.UpperLeft) &&
               skrrect_.GetRadii(SKRoundRectCorner.UpperRight) == other.skrrect_.GetRadii(SKRoundRectCorner.UpperRight) &&
               skrrect_.GetRadii(SKRoundRectCorner.LowerRight) == other.skrrect_.GetRadii(SKRoundRectCorner.LowerRight) &&
               skrrect_.GetRadii(SKRoundRectCorner.LowerLeft) == other.skrrect_.GetRadii(SKRoundRectCorner.LowerLeft);
    }

    public static bool operator == (in RRectF left, in RRectF right) => left.Equals(right);
    public static bool operator != (in RRectF left, in RRectF right) => !left.Equals(right);

    public void operator +=(in Vector2DF offset)
    {
        Offset(offset.X, offset.Y);
    }

    public void operator -=(in Vector2DF offset)
    {
        Offset(-offset.X, -offset.Y);
    }

    public static RRectF operator +(in RRectF a, in Vector2DF b)
    {
        RRectF result = a;
        result += b;
        return result;
    }

    public static RRectF operator -(in RRectF a, in Vector2DF b)
    {
        RRectF result = a;
        result -= b;
        return result;
    }
}
