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
        kUpperLeft  = SKRoundRectCorner.UpperLeft,
        kUpperRight = SKRoundRectCorner.UpperRight,
        kLowerRight = SKRoundRectCorner.LowerRight,
        kLowerLeft  = SKRoundRectCorner.LowerLeft
    }

    // Plain value-type fields — no heap allocation, true struct copy semantics.
    private SKRect  _rect;
    private SKPoint _radiiUpperLeft;
    private SKPoint _radiiUpperRight;
    private SKPoint _radiiLowerRight;
    private SKPoint _radiiLowerLeft;

    public RRectF() { }

    public RRectF(in SKRoundRect rrect)
    {
        _rect           = rrect.Rect;
        _radiiUpperLeft  = rrect.GetRadii(SKRoundRectCorner.UpperLeft);
        _radiiUpperRight = rrect.GetRadii(SKRoundRectCorner.UpperRight);
        _radiiLowerRight = rrect.GetRadii(SKRoundRectCorner.LowerRight);
        _radiiLowerLeft  = rrect.GetRadii(SKRoundRectCorner.LowerLeft);
        Normalize();
    }

    public RRectF(in RectF rect) : this(rect, 0.0f) {}

    public RRectF(in RectF rect, float radius) : this(rect, radius, radius) {}

    public RRectF(in RectF rect, float x_rad, float y_rad)
        : this(rect.X, rect.Y, rect.Width, rect.Height, x_rad, y_rad) {}

    // Sets all x and y radii to radius.
    public RRectF(float x, float y, float width, float height, float radius)
        : this(x, y, width, height, radius, radius) {}

    // Sets all x radii to x_rad, and all y radii to y_rad.
    // If one of x_rad or y_rad are zero, sets ALL radii to zero.
    public RRectF(float x, float y, float width, float height, float x_rad, float y_rad)
    {
        // SKRect takes left, top, right, bottom.
        _rect = new SKRect(x, y, x + width, y + height);

        // If either radius component is zero, all radii are zeroed (matching Skia behaviour).
        if (x_rad == 0 || y_rad == 0)
            x_rad = y_rad = 0;

        _radiiUpperLeft  = new SKPoint(x_rad, y_rad);
        _radiiUpperRight = new SKPoint(x_rad, y_rad);
        _radiiLowerRight = new SKPoint(x_rad, y_rad);
        _radiiLowerLeft  = new SKPoint(x_rad, y_rad);
        Normalize();
    }

    // Directly sets all four corners.
    public RRectF(float x, float y, float width, float height,
                  float upper_left_x,  float upper_left_y,
                  float upper_right_x, float upper_right_y,
                  float lower_right_x, float lower_right_y,
                  float lower_left_x,  float lower_left_y)
    {
        _rect            = new SKRect(x, y, x + width, y + height);
        _radiiUpperLeft  = new SKPoint(upper_left_x,  upper_left_y);
        _radiiUpperRight = new SKPoint(upper_right_x, upper_right_y);
        _radiiLowerRight = new SKPoint(lower_right_x, lower_right_y);
        _radiiLowerLeft  = new SKPoint(lower_left_x,  lower_left_y);
        Normalize();
    }

    public RRectF(in RectF rect,
                  float upper_left_x,  float upper_left_y,
                  float upper_right_x, float upper_right_y,
                  float lower_right_x, float lower_right_y,
                  float lower_left_x,  float lower_left_y)
        : this(rect.X, rect.Y, rect.Width, rect.Height,
               upper_left_x,  upper_left_y,
               upper_right_x, upper_right_y,
               lower_right_x, lower_right_y,
               lower_left_x,  lower_left_y) { }

    RRectF(in RectF rect, in RoundedCornersF corners)
        : this(rect.X, rect.Y, rect.Width, rect.Height,
               corners.UpperLeft,  corners.UpperLeft,
               corners.UpperRight, corners.UpperRight,
               corners.LowerRight, corners.LowerRight,
               corners.LowerLeft,  corners.LowerLeft) { }

    private static SKPoint ClampRadii(SKPoint r, float halfW, float halfH)
    {
        if (r.X <= 0 || r.Y <= 0) return SKPoint.Empty;
        float scale = Math.Min(halfW / r.X, Math.Min(halfH / r.Y, 1f));
        return new SKPoint(r.X * scale, r.Y * scale);
    }

    // Clears all fields if the rect is empty, mirroring Skia's normalisation.
    private void Normalize()
    {
        if (RectIsEmpty)
        {
            this = default;
            return;
        }

        // Clamp radii so they fit within the rect, matching Skia's behaviour.
        // When a radius exceeds half its dimension, both x and y are scaled down
        // proportionally by the most constraining dimension.
        float halfW = _rect.Width / 2f;
        float halfH = _rect.Height / 2f;

        // For uniform radii (simple case), scale both by the same factor.
        if (_radiiUpperLeft == _radiiUpperRight &&
            _radiiUpperLeft == _radiiLowerRight &&
            _radiiUpperLeft == _radiiLowerLeft)
        {
            float rx = _radiiUpperLeft.X;
            float ry = _radiiUpperLeft.Y;
            if (rx > 0 && ry > 0)
            {
                float scale = Math.Min(halfW / rx, halfH / ry);
                if (scale < 1f)
                {
                    var clamped = new SKPoint(rx * scale, ry * scale);
                    _radiiUpperLeft = clamped;
                    _radiiUpperRight = clamped;
                    _radiiLowerRight = clamped;
                    _radiiLowerLeft = clamped;
                }
            }
        }
        else
        {
            // For complex radii, clamp each corner independently.
            _radiiUpperLeft = ClampRadii(_radiiUpperLeft, halfW, halfH);
            _radiiUpperRight = ClampRadii(_radiiUpperRight, halfW, halfH);
            _radiiLowerRight = ClampRadii(_radiiLowerRight, halfW, halfH);
            _radiiLowerLeft = ClampRadii(_radiiLowerLeft, halfW, halfH);
        }
    }

    public readonly RoundRectType GetRoundRectType()
    {
        if (RectIsEmpty)
            return RoundRectType.kEmpty;

        bool allZero = _radiiUpperLeft  == SKPoint.Empty &&
                       _radiiUpperRight == SKPoint.Empty &&
                       _radiiLowerRight == SKPoint.Empty &&
                       _radiiLowerLeft  == SKPoint.Empty;

        if (allZero)
            return RoundRectType.kRect;

        bool allEqual = _radiiUpperLeft  == _radiiUpperRight &&
                        _radiiUpperLeft  == _radiiLowerRight &&
                        _radiiUpperLeft  == _radiiLowerLeft;

        if (allEqual)
        {
            // Check for oval: radii match half the rect dimensions.
            float halfW = _rect.Width  / 2f;
            float halfH = _rect.Height / 2f;
            if (_radiiUpperLeft.X == halfW && _radiiUpperLeft.Y == halfH)
                return _radiiUpperLeft.X == _radiiUpperLeft.Y
                    ? RoundRectType.kSingle
                    : RoundRectType.kOval;

            return _radiiUpperLeft.X == _radiiUpperLeft.Y
                ? RoundRectType.kSingle
                : RoundRectType.kSimple;
        }

        return RoundRectType.kComplex;
    }

    // The rectangular portion of the RRectF, without the corner radii.
    public readonly RectF rect() => SkRectToRectF(_rect);

    // Returns the radii of all corners. DCHECKs that all corners
    // have the same radii (the type is <= kOval).
    public readonly Vector2DF GetSimpleRadii()
    {
#if DEBUG
        Debug.Assert(GetRoundRectType() <= RoundRectType.kOval);
#endif
        return new Vector2DF(_radiiUpperLeft.X, _radiiUpperLeft.Y);
    }

    // Returns the radius of all corners. DCHECKs that all corners have the same
    // radii, and that x_rad == y_rad (the type is <= kSingle).
    public readonly float GetSimpleRadius()
    {
#if DEBUG
        Debug.Assert(GetRoundRectType() <= RoundRectType.kSingle);
        Debug.Assert(_radiiUpperLeft.X == _radiiUpperLeft.Y);
#endif
        return _radiiUpperLeft.X;
    }

    // Make the RRectF empty.
    public void Clear() => this = default;

    private readonly bool RectIsEmpty => _rect.Width <= 0 || _rect.Height <= 0;

    public readonly bool IsEmpty() => RectIsEmpty;

    public readonly bool HasRoundedCorners() =>
        !IsEmpty() && GetRoundRectType() != RoundRectType.kRect;

    public readonly Vector2DF GetCornerRadii(RoundRectCorner corner)
    {
        SKPoint r = corner switch
        {
            RoundRectCorner.kUpperLeft  => _radiiUpperLeft,
            RoundRectCorner.kUpperRight => _radiiUpperRight,
            RoundRectCorner.kLowerRight => _radiiLowerRight,
            RoundRectCorner.kLowerLeft  => _radiiLowerLeft,
            _ => SKPoint.Empty
        };
        return new Vector2DF(r.X, r.Y);
    }

    public void SetCornerRadii(RoundRectCorner corner, float x_rad, float y_rad)
    {
        var pt = new SKPoint(x_rad, y_rad);
        switch (corner)
        {
            case RoundRectCorner.kUpperLeft:  _radiiUpperLeft  = pt; break;
            case RoundRectCorner.kUpperRight: _radiiUpperRight = pt; break;
            case RoundRectCorner.kLowerRight: _radiiLowerRight = pt; break;
            case RoundRectCorner.kLowerLeft:  _radiiLowerLeft  = pt; break;
        }
    }

    public void SetCornerRadii(RoundRectCorner corner, in Vector2DF radii) => SetCornerRadii(corner, radii.X, radii.Y);

    // Returns true if |rect| is inside the bounds and corner radii of this
    // RRectF, and if both this RRectF and rect are not empty.
    public readonly bool Contains(in RectF rect)
    {
        using var rrect = ToSKRoundRect();
        return rrect.Contains(RectFToSkRect(rect));
    }

    // Move the rectangle by a horizontal and vertical distance.
    public void Offset(float horizontal, float vertical)
    {
        _rect.Left   += horizontal;
        _rect.Right  += horizontal;
        _rect.Top    += vertical;
        _rect.Bottom += vertical;
    }

    public void Offset(in Vector2DF distance) => Offset(distance.X, distance.Y);

    // Creates an SKRoundRect for use with Skia APIs. The caller is responsible
    // for disposing the returned object.
    public readonly SKRoundRect ToSKRoundRect()
    {
        var rrect = new SKRoundRect();

        if (!RectIsEmpty)
        {
            Span<SKPoint> radii = stackalloc SKPoint[]
            {
                _radiiUpperLeft, _radiiUpperRight,
                _radiiLowerRight, _radiiLowerLeft
            };
            rrect.SetRectRadii(_rect, radii);
        }
        return rrect;
    }

    public readonly bool Equals(in RRectF other)
    {
        // Two empty rects are always equal regardless of their original geometry.
        bool thisEmpty  = RectIsEmpty;
        bool otherEmpty = other.RectIsEmpty;

        if (thisEmpty && otherEmpty) return true;
        if (thisEmpty != otherEmpty) return false;

        return _rect            == other._rect            &&
               _radiiUpperLeft  == other._radiiUpperLeft  &&
               _radiiUpperRight == other._radiiUpperRight &&
               _radiiLowerRight == other._radiiLowerRight &&
               _radiiLowerLeft  == other._radiiLowerLeft;
    }

    public override readonly bool Equals(object? obj) => obj is RRectF other && Equals(other);

    public override readonly int GetHashCode() =>
        HashCode.Combine(_rect.Left, _rect.Top, _rect.Right, _rect.Bottom,
                         _radiiUpperLeft, _radiiUpperRight, _radiiLowerRight, _radiiLowerLeft);

    public static bool operator ==(in RRectF left, in RRectF right) => left.Equals(right);
    public static bool operator !=(in RRectF left, in RRectF right) => !left.Equals(right);

    public void operator +=(in Vector2DF offset) => Offset(offset.X, offset.Y);

    public void operator -=(in Vector2DF offset) => Offset(-offset.X, -offset.Y);

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
