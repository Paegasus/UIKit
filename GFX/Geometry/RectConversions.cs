using System.Diagnostics;

using static UI.Numerics.SafeConversions;

namespace UI.GFX.Geometry;

public static class RectConversions
{
    static int FloorIgnoringError(float f, float error)
    {
        int rounded = ClampRound(f);

        return Math.Abs(rounded - f) < error ? rounded : ClampFloor(f);
    }

    static int CeilIgnoringError(float f, float error)
    {
        int rounded = ClampRound(f);

        return Math.Abs(rounded - f) < error ? rounded : ClampCeil(f);
    }

    // Returns the smallest Rect that encloses the given RectF if possible.
    // The returned Rect is larger than or equal to the input RectF, unless the
    // the geometry values exceed int range and are clamped to int.
    public static Rect ToEnclosingRect(in RectF rect)
    {
        int left = ClampFloor(rect.X);
        int right = rect.Width != 0f ? ClampCeil(rect.Right) : left;
        int top = ClampFloor(rect.Y);
        int bottom = rect.Height != 0f ? ClampCeil(rect.Bottom) : top;

        Rect result = new();
        result.SetByBounds(left, top, right, bottom);
        return result;
    }

    // Similar to ToEnclosingRect(), but for each edge, if the distance between the
    // edge and the nearest integer grid is smaller than |error|, the edge is
    // snapped to the integer grid. Unlike ToNearestRect() which only accepts
    // integer rect with or without floating point error, this function also accepts
    // non-integer rect. The default error is 0.001, which is used in cc/viz.
    public static Rect ToEnclosingRectIgnoringError(in RectF rect, float error = 0.001f)
    {
        int left = FloorIgnoringError(rect.X, error);
        int right = rect.Width != 0f ? CeilIgnoringError(rect.Right, error) : left;
        int top = FloorIgnoringError(rect.Y, error);
        int bottom = rect.Height != 0f ? CeilIgnoringError(rect.Bottom, error) : top;

        Rect result = new();
        result.SetByBounds(left, top, right, bottom);
        return result;
    }

    // Returns the largest Rect that is enclosed by the given RectF if possible.
    // The returned rect is smaller than or equal to the input rect, but if
    // the input RectF is too small and no enclosed Rect exists, the returned
    // rect is an empty Rect at |ToCeiledPoint(rect.origin())|.
    public static Rect ToEnclosedRect(in RectF rect)
    {
        Rect result = new();
        result.SetByBounds(ClampCeil(rect.X), ClampCeil(rect.Y),
                           ClampFloor(rect.Right),
                           ClampFloor(rect.Bottom));
        return result;
    }

    // Similar to ToEnclosedRect(), but for each edge, if the distance between the
    // edge and the nearest integer grid is smaller than |error|, the edge is
    // snapped to the integer grid. Unlike ToNearestRect() which only accepts
    // integer rect with or without floating point error, this function also accepts
    // non-integer rect.
    public static Rect ToEnclosedRectIgnoringError(in RectF rect, float error)
    {
        int left = CeilIgnoringError(rect.X, error);
        int right = rect.Width != 0f ? FloorIgnoringError(rect.Right, error) : left;
        int top = CeilIgnoringError(rect.Y, error);
        int bottom = rect.Height != 0f ? FloorIgnoringError(rect.Bottom, error) : top;

        Rect result = new();
        result.SetByBounds(left, top, right, bottom);
        return result;
    }

    // Returns the Rect after snapping the corners of the RectF to an integer grid.
    // This should only be used when the RectF you provide is expected to be an
    // integer rect with floating point error. If it is an arbitrary RectF, then
    // you should use a different method.
    public static Rect ToNearestRect(in RectF rect)
    {
        float float_min_x = rect.X;
        float float_min_y = rect.Y;
        float float_max_x = rect.Right;
        float float_max_y = rect.Bottom;

        int min_x = ClampRound(float_min_x);
        int min_y = ClampRound(float_min_y);
        int max_x = ClampRound(float_max_x);
        int max_y = ClampRound(float_max_y);

        // If these DCHECKs fail, you're using the wrong method, consider using
        // ToEnclosingRect or ToEnclosedRect instead.
#if DEBUG
        Debug.Assert(Math.Abs(min_x - float_min_x) < 0.01f);
        Debug.Assert(Math.Abs(min_y - float_min_y) < 0.01f);
        Debug.Assert(Math.Abs(max_x - float_max_x) < 0.01f);
        Debug.Assert(Math.Abs(max_y - float_max_y) < 0.01f);
#endif
        Rect result = new();
        result.SetByBounds(min_x, min_y, max_x, max_y);

        return result;
    }

    // Returns true if the Rect produced after snapping the corners of the RectF
    // to an integer grid is withing |distance|.
    public static bool IsNearestRectWithinDistance(in RectF rect, float distance)
    {
        float float_min_x = rect.X;
        float float_min_y = rect.Y;
        float float_max_x = rect.Right;
        float float_max_y = rect.Bottom;

        int min_x = ClampRound(float_min_x);
        int min_y = ClampRound(float_min_y);
        int max_x = ClampRound(float_max_x);
        int max_y = ClampRound(float_max_y);

        return (Math.Abs(min_x - float_min_x) < distance) &&
               (Math.Abs(min_y - float_min_y) < distance) &&
               (Math.Abs(max_x - float_max_x) < distance) &&
               (Math.Abs(max_y - float_max_y) < distance);
    }

    // Returns the Rect after rounding the corners of the RectF to an integer grid.
    public static Rect ToRoundedRect(in RectF rect)
    {
        int left = ClampRound(rect.X);
        int top = ClampRound(rect.Y);
        int right = ClampRound(rect.Right);
        int bottom = ClampRound(rect.Bottom);
        Rect result = new();
        result.SetByBounds(left, top, right, bottom);
        return result;
    }

    // Returns a Rect obtained by flooring the values of the given RectF.
    // Please prefer the previous two functions in new code.
    public static Rect ToFlooredRectDeprecated(in RectF rect)
    {
        return new Rect(ClampFloor(rect.X), ClampFloor(rect.Y),
              ClampFloor(rect.Width), ClampFloor(rect.Height));
    }
}
