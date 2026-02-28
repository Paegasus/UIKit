namespace UI.GFX.Geometry;

// This file contains helper functions to move between DIPs (device-independent pixels) and physical pixels,
// by multiplying or dividing by device scale factor.
// These help show the intent of the caller by naming the operation,
// instead of directly performing a scale operation. More complicated
// transformations between coordinate spaces than DIP<->physical pixels should
// be done via more explicit means.
//
// Note that functions that receive integer values will convert them to floating
// point values, which can itself be a lossy operation for large integers.
// The intention of these methods is to be used for UI values which are relatively small.

public static class Diputil
{
    public static PointF ConvertPointToDips(in Point point_in_pixels, float device_scale_factor)
    {
        return PointF.ScalePoint(new PointF(point_in_pixels), 1.0f / device_scale_factor);
    }

    public static PointF ConvertPointToDips(in PointF point_in_pixels, float device_scale_factor)
    {
        return PointF.ScalePoint(point_in_pixels, 1.0f / device_scale_factor);
    }

    public static PointF ConvertPointToPixels(in Point point_in_dips, float device_scale_factor)
    {
        return PointF.ScalePoint(new PointF(point_in_dips), device_scale_factor);
    }

    public static PointF ConvertPointToPixels(in PointF point_in_dips, float device_scale_factor)
    {
        return PointF.ScalePoint(point_in_dips, device_scale_factor);
    }

    public static SizeF ConvertSizeToDips(in Size size_in_pixels, float device_scale_factor)
    {
        return SizeF.ScaleSize(new SizeF(size_in_pixels), 1.0f / device_scale_factor);
    }

    public static SizeF ConvertSizeToDips(in SizeF size_in_pixels, float device_scale_factor)
    {
        return SizeF.ScaleSize(size_in_pixels, 1.0f / device_scale_factor);
    }

    public static SizeF ConvertSizeToPixels(in Size size_in_dips, float device_scale_factor)
    {
        return SizeF.ScaleSize(new SizeF(size_in_dips), device_scale_factor);
    }

    public static SizeF ConvertSizeToPixels(in SizeF size_in_dips, float device_scale_factor)
    {
        return SizeF.ScaleSize(size_in_dips, device_scale_factor);
    }

    public static RectF ConvertRectToDips(in Rect rect_in_pixels, float device_scale_factor)
    {
        return RectF.ScaleRect(new RectF(rect_in_pixels), 1.0f / device_scale_factor);
    }

    public static RectF ConvertRectToDips(in RectF rect_in_pixels, float device_scale_factor)
    {
        return RectF.ScaleRect(rect_in_pixels, 1.0f / device_scale_factor);
    }

    public static RectF ConvertRectToPixels(in Rect rect_in_dips, float device_scale_factor)
    {
        return RectF.ScaleRect(new RectF(rect_in_dips), device_scale_factor);
    }

    public static RectF ConvertRectToPixels(in RectF rect_in_dips, float device_scale_factor)
    {
        return RectF.ScaleRect(rect_in_dips, device_scale_factor);
    }

    public static InsetsF ConvertInsetsToDips(in Insets insets_in_pixels, float device_scale_factor)
    {
        return InsetsF.ScaleInsets(new InsetsF(insets_in_pixels), 1.0f / device_scale_factor);
    }

    public static InsetsF ConvertInsetsToDips(in InsetsF insets_in_pixels, float device_scale_factor)
    {
        return InsetsF.ScaleInsets(insets_in_pixels, 1.0f / device_scale_factor);
    }

    public static InsetsF ConvertInsetsToPixels(in Insets insets_in_dips, float device_scale_factor)
    {
        return InsetsF.ScaleInsets(new InsetsF(insets_in_dips), device_scale_factor);
    }

    public static InsetsF ConvertInsetsToPixels(in InsetsF insets_in_dips, float device_scale_factor)
    {
        return InsetsF.ScaleInsets(insets_in_dips, device_scale_factor);
    }
}
