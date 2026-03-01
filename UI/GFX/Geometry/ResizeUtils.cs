using System.Diagnostics;

using static UI.Numerics.SafeConversions;

namespace UI.GFX.Geometry;

public static class ResizeUtils
{
    public enum ResizeEdge
    {
        kBottom,
        kBottomLeft,
        kBottomRight,
        kLeft,
        kRight,
        kTop,
        kTopLeft,
        kTopRight
    }

    // This function decides whether SizeRectToAspectRatio() will adjust the height
    // to match the specified width (resizing horizontally) or vice versa (resizing vertically).
    public static bool IsResizingHorizontally(ResizeEdge resize_edge) => resize_edge switch
    {
        ResizeEdge.kLeft => true,
        ResizeEdge.kRight => true,
        ResizeEdge.kTopLeft => true,
        ResizeEdge.kBottomLeft => true,
        _ => false
    };

    // Updates |rect| to adhere to the |aspect_ratio| of the window, if it has
    // been set. |resize_edge| refers to the edge of the window being sized.
    // |min_window_size| and |max_window_size| are expected to adhere to the
    // given aspect ratio.
    // |aspect_ratio| must be valid and is found using width / height.
    public static void SizeRectToAspectRatio(ResizeEdge resize_edge, float aspect_ratio, in Size min_window_size, Size? max_window_size, ref Rect rect)
    {
        SizeRectToAspectRatioWithExcludedMargin(resize_edge, aspect_ratio, min_window_size, max_window_size, new Size(), ref rect);
    }

    // As above, but computes a size for `rect` such that it has the right aspect
    // ratio after subtracting `excluded_margin` from it.  This lets the aspect
    // ratio ignore fixed borders, like a title bar.  For example, if
    // `excluded_margin` is (10, 5) and `aspect_ratio` is 1.0f, then the resulting
    // rectangle might have a size of (30, 25) or (40, 35).  One could use the
    // margin for drawing in the edges, and the part that's left over would have the
    // proper aspect ratio: 20/20 or 30/30, respectively.
    public static void SizeRectToAspectRatioWithExcludedMargin(ResizeEdge resize_edge, float aspect_ratio, in Size original_min_window_size, Size? max_window_size, in Size excluded_margin, ref Rect rect)
    {
#if DEBUG
        Debug.Assert(aspect_ratio > 0.0f);

        if (max_window_size.HasValue)
        {
            Debug.Assert(max_window_size.Value.Width >= original_min_window_size.Width);
            Debug.Assert(max_window_size.Value.Height >= original_min_window_size.Height);
            Debug.Assert(max_window_size.Value.Width >= excluded_margin.Width);
            Debug.Assert(max_window_size.Value.Height >= excluded_margin.Height);
            Debug.Assert(new Rect(rect.Origin, max_window_size.Value).Contains(rect));
        }

        Debug.Assert(rect.Contains(new Rect(rect.Origin, original_min_window_size)));
#endif
        // Compute the aspect ratio with the excluded margin removed from both the
        // rectangle and the maximum size. Note that the edge we ask for doesn't
        // really matter; we'll position the resulting rectangle correctly later.
        Size new_size = new(rect.Width - excluded_margin.Width,
                            rect.Height - excluded_margin.Height);

        if (max_window_size.HasValue)
        {
            max_window_size = new Size(max_window_size.Value.Width - excluded_margin.Width,
                                       max_window_size.Value.Height - excluded_margin.Height);
        }

        // Also remove the margin from the minimum size, since it'll get added back at the end.
        Size min_window_size = original_min_window_size - excluded_margin;

        if (IsResizingHorizontally(resize_edge))
        {
            new_size.Height = ClampRound(new_size.Width / aspect_ratio);
            
            if (min_window_size.Height > new_size.Height ||
                (max_window_size.HasValue && new_size.Height > max_window_size.Value.Height))
            {
                new_size.Height = max_window_size.HasValue
                    ? Math.Clamp(new_size.Height, min_window_size.Height, max_window_size.Value.Height)
                    : min_window_size.Height;
                
                new_size.Width = ClampRound(new_size.Height * aspect_ratio);
            }
        }
        else
        {
            new_size.Width = ClampRound(new_size.Height * aspect_ratio);

            if (min_window_size.Width > new_size.Width ||
                (max_window_size.HasValue && new_size.Width > max_window_size.Value.Width))
            {
                new_size.Width = max_window_size.HasValue
                    ? Math.Clamp(new_size.Width, min_window_size.Width, max_window_size.Value.Width)
                    : min_window_size.Width;
                
                new_size.Height = ClampRound(new_size.Width / aspect_ratio);
            }
        }

        // The dimensions might still be outside of the allowed ranges at this point.
        // This happens when the aspect ratio makes it impossible to fit |rect|
        // within the size limits without letter-/pillarboxing.
        if (max_window_size.HasValue)
            new_size.SetToMin(max_window_size.Value);
        
        // The minimum size also excludes any excluded margin, so the content area has
        // to make up the adjusted difference.
        new_size.SetToMax(min_window_size);

        // Now add the excluded margin back to the total size, so that the total size
        // is aligned with the resize edge.
        new_size.Enlarge(excluded_margin.Width, excluded_margin.Height);

        // |rect| bounds before sizing to aspect ratio.
        int left = rect.X;
        int top = rect.Y;
        int right = rect.Right;
        int bottom = rect.Bottom;

        switch (resize_edge)
        {
            case ResizeEdge.kRight:
            case ResizeEdge.kBottom:
                right = new_size.Width + left;
                bottom = top + new_size.Height;
                break;
            case ResizeEdge.kTop:
                right = new_size.Width + left;
                top = bottom - new_size.Height;
                break;
            case ResizeEdge.kLeft:
            case ResizeEdge.kTopLeft:
                left = right - new_size.Width;
                top = bottom - new_size.Height;
                break;
            case ResizeEdge.kTopRight:
                right = left + new_size.Width;
                top = bottom - new_size.Height;
                break;
            case ResizeEdge.kBottomLeft:
                left = right - new_size.Width;
                bottom = top + new_size.Height;
                break;
            case ResizeEdge.kBottomRight:
                right = left + new_size.Width;
                bottom = top + new_size.Height;
                break;
        }

        rect.SetByBounds(left, top, right, bottom);
    }
}
