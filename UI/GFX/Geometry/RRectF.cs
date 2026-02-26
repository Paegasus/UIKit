using System.Diagnostics;
using System.Text;
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
    public SKRect _rect;
    public SKPoint _radiiUpperLeft;
    public SKPoint _radiiUpperRight;
    public SKPoint _radiiLowerRight;
    public SKPoint _radiiLowerLeft;

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

    public RRectF(in RectF rect, in RoundedCornersF corners)
        : this(rect.X, rect.Y, rect.Width, rect.Height,
               corners.UpperLeft,  corners.UpperLeft,
               corners.UpperRight, corners.UpperRight,
               corners.LowerRight, corners.LowerRight,
               corners.LowerLeft,  corners.LowerLeft) { }
    
    // The rectangular portion of the RRectF, without the corner radii.
    public readonly RectF Rect() => new(_rect.Left, _rect.Top, _rect.Width, _rect.Height);

    private static SKPoint ClampRadii(SKPoint r, float halfW, float halfH)
    {
        if (r.X <= 0 || r.Y <= 0) return SKPoint.Empty;
        float scale = Math.Min(halfW / r.X, Math.Min(halfH / r.Y, 1f));
        return new SKPoint(r.X * scale, r.Y * scale);
    }

    // Clears all fields if the rect is empty, mirroring Skia's normalisation.
    private void Normalize()
    {
        if (RectIsEmpty || !float.IsFinite(_rect.Left) || !float.IsFinite(_rect.Top) || !float.IsFinite(_rect.Right) || !float.IsFinite(_rect.Bottom))
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

    // Direct port of Skia's SkRRect::checkCornerContainment().
    // Assumes the point is already known to be inside the bounding rect.
    private readonly bool CheckCornerContainment(float x, float y)
    {
        SKPoint canonicalPt;
        SKPoint radii;

        if (x < _rect.Left + _radiiUpperLeft.X && y < _rect.Top + _radiiUpperLeft.Y)
        {
            canonicalPt = new SKPoint(x - (_rect.Left + _radiiUpperLeft.X),
                                      y - (_rect.Top + _radiiUpperLeft.Y));
            radii = _radiiUpperLeft;
        }
        else if (x < _rect.Left + _radiiLowerLeft.X && y > _rect.Bottom - _radiiLowerLeft.Y)
        {
            canonicalPt = new SKPoint(x - (_rect.Left + _radiiLowerLeft.X),
                                      y - (_rect.Bottom - _radiiLowerLeft.Y));
            radii = _radiiLowerLeft;
        }
        else if (x > _rect.Right - _radiiUpperRight.X && y < _rect.Top + _radiiUpperRight.Y)
        {
            canonicalPt = new SKPoint(x - (_rect.Right - _radiiUpperRight.X),
                                      y - (_rect.Top + _radiiUpperRight.Y));
            radii = _radiiUpperRight;
        }
        else if (x > _rect.Right - _radiiLowerRight.X && y > _rect.Bottom - _radiiLowerRight.Y)
        {
            canonicalPt = new SKPoint(x - (_rect.Right - _radiiLowerRight.X),
                                      y - (_rect.Bottom - _radiiLowerRight.Y));
            radii = _radiiLowerRight;
        }
        else
        {
            // Not in any corner region — point is on a straight edge, always inside.
            return true;
        }

        // b²x² + a²y² <= (ab)²  — avoids division, matches Skia exactly.
        float dist = canonicalPt.X * canonicalPt.X * radii.Y * radii.Y +
                     canonicalPt.Y * canonicalPt.Y * radii.X * radii.X;
        
        return dist <= radii.X * radii.X * radii.Y * radii.Y;
    }

    // Returns true if |rect| is inside the bounds and corner radii of this
    // RRectF, and if both this RRectF and rect are not empty.
    public readonly bool Contains(in RectF rect)
    {
        if (IsEmpty() || rect.IsEmpty())
            return false;

        SKRect r = new(rect.X, rect.Y, rect.Right, rect.Bottom);

        if (!_rect.Contains(r))
            return false;

        // A plain rect only needed the bounds check above.
        if (GetRoundRectType() == RoundRectType.kRect)
            return true;

        return CheckCornerContainment(r.Left, r.Top) &&
               CheckCornerContainment(r.Right, r.Top) &&
               CheckCornerContainment(r.Right, r.Bottom) &&
               CheckCornerContainment(r.Left, r.Bottom);
    }

    // Returns the bounding box that contains the specified rounded corner.
    public readonly RectF CornerBoundingRect(RoundRectCorner corner)
    {
        throw new NotImplementedException();
    }

    public void Scale(float x_scale, float y_scale)
    {
        if (IsEmpty())
            return;

        if (x_scale == 0 || y_scale == 0)
        {
            this = default;
            return;
        }

        _rect = new SKRect(_rect.Left * x_scale,
                           _rect.Top * y_scale,
                           _rect.Right * x_scale,
                           _rect.Bottom * y_scale);

        _radiiUpperLeft = new SKPoint(_radiiUpperLeft.X * x_scale, _radiiUpperLeft.Y * y_scale);
        _radiiUpperRight = new SKPoint(_radiiUpperRight.X * x_scale, _radiiUpperRight.Y * y_scale);
        _radiiLowerRight = new SKPoint(_radiiLowerRight.X * x_scale, _radiiLowerRight.Y * y_scale);
        _radiiLowerLeft = new SKPoint(_radiiLowerLeft.X * x_scale, _radiiLowerLeft.Y * y_scale);

        Normalize();
    }

    // Scales the rectangle by |scale|.
    public void Scale(float scale) => Scale(scale, scale);

    // Move the rectangle by a horizontal and vertical distance.
    public void Offset(float horizontal, float vertical)
    {
        _rect.Left   += horizontal;
        _rect.Right  += horizontal;
        _rect.Top    += vertical;
        _rect.Bottom += vertical;
    }

    public void Offset(in Vector2DF distance) => Offset(distance.X, distance.Y);

    // Insets bounds by dx and dy, and adjusts radii by dx and dy. dx and dy may
    // be positive, negative, or zero. If either corner radius is zero, the corner
    // has no curvature and is unchanged. Otherwise, if adjusted radius becomes
    // negative, the radius is pinned to zero.
    public void Inset(float val) => Inset(val, val);

    public void Inset(float dx, float dy)
    {
        SKRect r = new(_rect.Left + dx,
                       _rect.Top + dy,
                       _rect.Right - dx,
                       _rect.Bottom - dy);

        bool degenerate = false;

        if (r.Right <= r.Left)
        {
            degenerate = true;
            float mid = (r.Left + r.Right) * 0.5f;
            r.Left = r.Right = mid;
        }
        if (r.Bottom <= r.Top)
        {
            degenerate = true;
            float mid = (r.Top + r.Bottom) * 0.5f;
            r.Top = r.Bottom = mid;
        }

        if (degenerate)
        {
            this = default;
            return;
        }

        if (!float.IsFinite(r.Left) || !float.IsFinite(r.Top) ||
            !float.IsFinite(r.Right) || !float.IsFinite(r.Bottom))
        {
            this = default;
            return;
        }

        // Only adjust a radius component if it was non-zero, then clamp to zero.
        // This matches Skia: zero radius means square corner, leave it unchanged.
        _rect = r;
        if (_radiiUpperLeft.X != 0) _radiiUpperLeft.X = MathF.Max(0, _radiiUpperLeft.X - dx);
        if (_radiiUpperLeft.Y != 0) _radiiUpperLeft.Y = MathF.Max(0, _radiiUpperLeft.Y - dy);
        if (_radiiUpperRight.X != 0) _radiiUpperRight.X = MathF.Max(0, _radiiUpperRight.X - dx);
        if (_radiiUpperRight.Y != 0) _radiiUpperRight.Y = MathF.Max(0, _radiiUpperRight.Y - dy);
        if (_radiiLowerRight.X != 0) _radiiLowerRight.X = MathF.Max(0, _radiiLowerRight.X - dx);
        if (_radiiLowerRight.Y != 0) _radiiLowerRight.Y = MathF.Max(0, _radiiLowerRight.Y - dy);
        if (_radiiLowerLeft.X != 0) _radiiLowerLeft.X = MathF.Max(0, _radiiLowerLeft.X - dx);
        if (_radiiLowerLeft.Y != 0) _radiiLowerLeft.Y = MathF.Max(0, _radiiLowerLeft.Y - dy);

        Normalize();
    }

    // Outsets bounds by dx and dy, and adjusts radii by dx and dy. dx and dy may
    // be positive, negative, or zero. If either corner radius is zero, the corner
    // has no curvature and is unchanged. Otherwise, if adjusted radius becomes
    // negative, the radius is pinned to zero.
    /*
    void Outset(float val)
    {
        skrrect_.outset(val, val);
    }

    void Outset(float dx, float dy)
    {
        skrrect_.outset(dx, dy);
    }
    */

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

    public override readonly string ToString()
    {
        var ss = new StringBuilder();

        var rect = Rect();

        ss.AppendFormat("{0:0.000}", rect.Origin.X);
        ss.Append(',');
        ss.AppendFormat("{0:0.000}", rect.Origin.Y);
        ss.Append(' ');
        ss.AppendFormat("{0:0.000}", rect.Size.Width);
        ss.Append('x');
        ss.AppendFormat("{0:0.000}", rect.Size.Height);

        var type = GetRoundRectType();

        if (type <= RoundRectType.kRect)
        {
            ss.Append(", rectangular");
        }
        else if (type <= RoundRectType.kSingle)
        {
            ss.Append(", radius ");
            ss.AppendFormat("{0:0.000}", GetSimpleRadius());
        }
        else if (type <= RoundRectType.kSimple)
        {
            var radii = GetSimpleRadii();

            ss.Append(", x_rad ");
            ss.AppendFormat("{0:0.000}", radii.X);
            ss.Append(", y_rad ");
            ss.AppendFormat("{0:0.000}", radii.Y);
        }
        else
        {
            ss.Append(',');

            RoundRectCorner[] corners = [RoundRectCorner.kUpperLeft,
                                         RoundRectCorner.kUpperRight,
                                         RoundRectCorner.kLowerRight,
                                         RoundRectCorner.kLowerLeft];
            
            foreach (var c in corners)
            {
                var corner = GetCornerRadii(c);

                
                ss.Append(" [");
                ss.AppendFormat("{0:0.000}", corner.X);
                ss.Append(' ');
                ss.AppendFormat("{0:0.000}", corner.Y);
                ss.Append(']');
            }
        }

        return ss.ToString();
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
