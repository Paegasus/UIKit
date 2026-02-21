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

    private readonly float MapX(float x)
    {
        return ClampFloatGeometry(x * scale_.X + translation_.X);
    }
    private readonly float MapY(float y)
    {
        return ClampFloatGeometry(y * scale_.Y + translation_.Y);
    }
    private readonly float InverseMapX(float x)
    {
        // |* (1.f / scale)| instead of '/ scale' to keep the same precision
        // before crrev.com/c/3937107.
        return ClampFloatGeometry((x - translation_.X) * (1.0f / scale_.X));
    }
    private readonly float InverseMapY(float y)
    {
        // |* (1.f / scale)| instead of '/ scale' to keep the same precision
        // before crrev.com/c/3937107.
        return ClampFloatGeometry((y - translation_.Y) * (1.0f / scale_.Y));
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
            // If both scales are negative,
            // decompose to positive scales with a 180deg rotation.
            decomp.Scale.X = -scale_.X;
            decomp.Scale.Y = -scale_.Y;
            decomp.Quaternion.Z = 1;
            decomp.Quaternion.W = 0;
        }
        return decomp;
    }

    public override readonly string ToString()
    {
        return $"[{scale_}, {translation_}]";
    }
}
