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

    private SKRoundRect skrrect_;

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
        skrrect_ = new SKRoundRect(new SKRect(x, y, width, height), x_rad, y_rad);

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

    public readonly bool IsEmpty()
    {
        return GetRoundRectType() == RoundRectType.kEmpty;
    }

    public readonly bool HasRoundedCorners()
    {
        return !IsEmpty() && GetRoundRectType() != RoundRectType.kRect;
    }    
}
