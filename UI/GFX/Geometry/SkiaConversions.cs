using System.Diagnostics;
using SkiaSharp;

namespace UI.GFX.Geometry;

public static class SkiaConversions
{
    // Convert between Skia and GFX types.
    public static SKPoint PointToSkPoint(in Point point)
    {
        return new SKPoint(point.X, point.Y);
    }

    public static SKPointI PointToSkIPoint(in Point point)
    {
        return new SKPointI(point.X, point.Y);
    }
    
    public static Point SkIPointToPoint(in SKPointI point)
    {
        return new Point(point.X, point.Y);
    }
    
    public static SKPoint PointFToSkPoint(in PointF point)
    {
        return new SKPoint(point.X, point.Y);
    }
    
    public static PointF SkPointToPointF(in SKPoint point)
    {
        return new PointF(point.X, point.Y);
    }
    
    public static SKRect RectToSkRect(in Rect rect)
    {
        return new SKRect(rect.X, rect.Y, rect.Right, rect.Bottom);
    }
    
    public static SKRectI RectToSkIRect(in Rect rect)
    {
        return new SKRectI(rect.X, rect.Y, rect.Right, rect.Bottom);
    }
    
    public static Rect SkIRectToRect(in SKRectI rect)
    {
        Rect result = new();
        result.SetByBounds(rect.Left, rect.Top, rect.Right, rect.Bottom);
        return result;
    }
    
    public static SKRect RectFToSkRect(in RectF rect)
    {
        return new SKRect(rect.X, rect.Y, rect.Right, rect.Bottom);
    }
    
    public static RectF SkRectToRectF(in SKRect rect)
    {
        return RectF.BoundingRect(new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Bottom));
    }
    
    public static SKSize SizeFToSkSize(in SizeF size)
    {
        return new SKSize(size.Width, size.Height);
    }
    
    public static SKSizeI SizeToSkISize(in Size size)
    {
        return new SKSizeI(size.Width, size.Height);
    }
    
    public static SizeF SkSizeToSizeF(in SKSize size)
    {
        return new SizeF(size.Width, size.Height);
    }
    
    public static Size SkISizeToSize(in SKSizeI size)
    {
        return new Size(size.Width, size.Height);
    }
    
    public static void QuadFToSkPoints(in QuadF quad, Span<SKPoint> points)
    {
#if DEBUG
        Debug.Assert(points.Length >= 4);
#endif
        points[0] = PointFToSkPoint(quad.p1);
        points[1] = PointFToSkPoint(quad.p2);
        points[2] = PointFToSkPoint(quad.p3);
        points[3] = PointFToSkPoint(quad.p4);
    }

    public static SKMatrix AxisTransform2dToSkMatrix(in AxisTransform2D transform)
    {
        return new SKMatrix(
        transform.Scale.X, 0, transform.Translation.X,  // row 0
        0, transform.Scale.Y, transform.Translation.Y,  // row 1
        0, 0, 1);
    }

    public static SKMatrix44 TransformToSkM44(in Transform matrix)
    {
        // The parameters of this SkM44 constructor are in row-major order.
        return new SKMatrix44(
            (float)matrix.rc(0, 0), (float)matrix.rc(0, 1), (float)matrix.rc(0, 2), (float)matrix.rc(0, 3),
            (float)matrix.rc(1, 0), (float)matrix.rc(1, 1), (float)matrix.rc(1, 2), (float)matrix.rc(1, 3),
            (float)matrix.rc(2, 0), (float)matrix.rc(2, 1), (float)matrix.rc(2, 2), (float)matrix.rc(2, 3),
            (float)matrix.rc(3, 0), (float)matrix.rc(3, 1), (float)matrix.rc(3, 2), (float)matrix.rc(3, 3));
    }

    public static Transform SkM44ToTransform(in SKMatrix44 matrix)
    {
        // Note: SkiaSharp's SKMatrix44 indexer [i, j] uses column-major convention
        // (i = column, j = row), which is the opposite of Skia C++'s SkM44::rc(row, col).
        // So we swap the indices here to correctly read row, col.
        return Transform.RowMajor(
            matrix[0, 0], matrix[1, 0], matrix[2, 0], matrix[3, 0],
            matrix[0, 1], matrix[1, 1], matrix[2, 1], matrix[3, 1],
            matrix[0, 2], matrix[1, 2], matrix[2, 2], matrix[3, 2],
            matrix[0, 3], matrix[1, 3], matrix[2, 3], matrix[3, 3]);
    }

    public static SKMatrix TransformToFlattenedSkMatrix(Transform matrix)
    {
        // Convert from 4x4 to 3x3 by dropping row 2 (counted from 0) and column 2.
        return new SKMatrix((float)matrix.rc(0, 0), (float)matrix.rc(0, 1), (float)matrix.rc(0, 3),
                           (float)matrix.rc(1, 0), (float)matrix.rc(1, 1), (float)matrix.rc(1, 3),
                           (float)matrix.rc(3, 0), (float)matrix.rc(3, 1), (float)matrix.rc(3, 3));
    }

    public static Transform SkMatrixToTransform(in SKMatrix matrix)
    {
        return Transform.RowMajor(
        matrix.ScaleX, matrix.SkewX, 0, matrix.TransX,     // row 0
        matrix.SkewY, matrix.ScaleY, 0, matrix.TransY,     // row 1
        0, 0, 1, 0,                                        // row 2
        matrix.Persp0, matrix.Persp1, 0, matrix.Persp2);   // row 3
    }
}
