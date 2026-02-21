using System;
using System.Diagnostics;

using static UI.GFX.Geometry.ClampFloatGeometryHelper;

namespace UI.GFX.Geometry;

// This class implements the subset of 2D linear transforms that only
// translation and uniform scaling are allowed.
//
// Internally this is stored as a vector for pre-scale, and another vector for post-translation.
// The class constructor and member accessor follows the same convention,
// but a scalar scale factor is also accepted.
//
// Results of the *Map* methods are clamped with ClampFloatGeometry().
// See the definition of the function for details.
//
public struct AxisTransform2D
{
    // Scale is applied before translation, i.e.
    // this->Transform(p) == scale_ * p + translation_
    Vector2DF scale_;
    Vector2DF translation_;

    public readonly Vector2DF Scale => scale_;
    public readonly Vector2DF Translation => translation_;

    public AxisTransform2D()
    {
        scale_ = new(1.0f, 1.0f);
    }

    public AxisTransform2D(float scale, in Vector2DF translation)
    {
        scale_ = new(scale, scale);
        translation_ = translation;
    }

    private AxisTransform2D(in Vector2DF scale, in Vector2DF translation) => (scale_, translation_) = (scale, translation);

    public static AxisTransform2D FromScaleAndTranslation(in Vector2DF scale, in Vector2DF translation) => new(scale, translation);

    private readonly float MapX(float x) => ClampFloatGeometry(x * scale_.X + translation_.X);

    private readonly float MapY(float y) => ClampFloatGeometry(y * scale_.Y + translation_.Y);

    // |* (1.0f / scale)| instead of '/ scale' to keep the same precision before crrev.com/c/3937107.
    private readonly float InverseMapX(float x) => ClampFloatGeometry((x - translation_.X) * (1.0f / scale_.X));

    // |* (1.0f / scale)| instead of '/ scale' to keep the same precision before crrev.com/c/3937107.
    private readonly float InverseMapY(float y) => ClampFloatGeometry((y - translation_.Y) * (1.0f / scale_.Y));

    public void PreScale(in Vector2DF scale) => scale_.Scale(scale.X, scale.Y);

    public void PostScale(in Vector2DF scale)
    {
        scale_.Scale(scale.X, scale.Y);
        translation_.Scale(scale.X, scale.Y);
    }

    public void PreTranslate(in Vector2DF translation) => translation_ += Vector2DF.ScaleVector2D(translation, scale_.X, scale_.Y);

    public void PostTranslate(in Vector2DF translation) => translation_ += translation;

    public void PreConcat(in AxisTransform2D pre)
    {
        PreTranslate(pre.translation_);
        PreScale(pre.scale_);
    }
    public void PostConcat(in AxisTransform2D post)
    {
        PostScale(post.scale_);
        PostTranslate(post.translation_);
    }

    public readonly double Determinant() => (double)scale_.X * scale_.Y;

    // Check float determinant (stricter than checking each component or double
    // determinant) to keep consistency with Matrix44.
    // TODO(crbug.com/40237414): This may be stricter than necessary. Revisit
    // this after combination of gfx::Transform and blink::TransformationMatrix.
    public readonly bool IsInvertible() => float.IsNormal(scale_.X * scale_.Y);

    public void Invert()
    {
#if DEBUG
        Debug.Assert(IsInvertible());
#endif
        scale_ = new Vector2DF(1.0f / scale_.X, 1.0f / scale_.Y);
        translation_.Scale(-scale_.X, -scale_.Y);
    }

    // Changes the transform to: scale(z) * mat * scale(1/z).
    // Useful for mapping zoomed points to their zoomed transformed result:
    // new_mat * (scale(z) * x) == scale(z) * (mat * x).
    public void Zoom(float zoom_factor) => translation_.Scale(zoom_factor);

    public readonly PointF MapPoint(in PointF p) => new(MapX(p.X), MapY(p.Y));

    public readonly PointF InverseMapPoint(in PointF p) => new(InverseMapX(p.X), InverseMapY(p.Y));

    public readonly RectF MapRect(in RectF r)
    {
#if DEBUG
        Debug.Assert(scale_.X >= 0.0f);
        Debug.Assert(scale_.Y >= 0.0f);
#endif
        return new RectF(MapX(r.X), MapY(r.Y), ClampFloatGeometry(r.Width * scale_.X), ClampFloatGeometry(r.Height * scale_.Y));
    }

    public readonly RectF InverseMapRect(in RectF r)
    {
#if DEBUG
        Debug.Assert(scale_.X > 0.0f);
        Debug.Assert(scale_.Y > 0.0f);
#endif
        // |* (1.0f / scale)| instead of '/ scale' to keep the same precision before crrev.com/c/3937107.
        return new RectF(InverseMapX(r.X), InverseMapY(r.Y), ClampFloatGeometry(r.Width * (1.0f / scale_.X)), ClampFloatGeometry(r.Height * (1.0f / scale_.Y)));
    }

    // Decomposes this transform into |decomp|, following the 2d decomposition
    // spec: https://www.w3.org/TR/css-transforms-1/#decomposing-a-2d-matrix.
    // It's a simplified version of Matrix44::Decompose2d().
    public readonly DecomposedTransform Decompose()
    {
        DecomposedTransform decomp = new();

        decomp.Translate.X = translation_.X;
        decomp.Translate.Y = translation_.Y;

        if (scale_.X >= 0 || scale_.Y >= 0)
        {
            decomp.Scale.X = scale_.X;
            decomp.Scale.Y = scale_.Y;
        }
        else
        {
            // If both scales are negative, decompose to positive scales with a 180deg rotation.
            decomp.Scale.X = -scale_.X;
            decomp.Scale.Y = -scale_.Y;
            decomp.Quaternion.Z = 1;
            decomp.Quaternion.W = 0;
        }
        return decomp;
    }

    public static AxisTransform2D PreScaleAxisTransform2D(in AxisTransform2D t, float scale)
    {
        AxisTransform2D result = t;
        result.PreScale(new Vector2DF(scale, scale));
        return result;
    }

    public static AxisTransform2D PostScaleAxisTransform2D(in AxisTransform2D t, float scale)
    {
        AxisTransform2D result = t;
        result.PostScale(new Vector2DF(scale, scale));
        return result;
    }

    public static AxisTransform2D PreTranslateAxisTransform2D(in AxisTransform2D t, in Vector2DF translation)
    {
        AxisTransform2D result = t;
        result.PreTranslate(translation);
        return result;
    }

    public static AxisTransform2D PostTranslateAxisTransform2D(in AxisTransform2D t, in Vector2DF translation)
    {
        AxisTransform2D result = t;
        result.PostTranslate(translation);
        return result;
    }

    public static AxisTransform2D ConcatAxisTransform2D(in AxisTransform2D post, in AxisTransform2D pre)
    {
        AxisTransform2D result = post;
        result.PreConcat(pre);
        return result;
    }

    public static AxisTransform2D InvertAxisTransform2D(in AxisTransform2D t)
    {
        AxisTransform2D result = t;
        result.Invert();
        return result;
    }

    public override readonly string ToString() => $"[{scale_}, {translation_}]";
}
