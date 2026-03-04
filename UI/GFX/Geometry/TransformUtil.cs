// Copyright 2012 The Chromium Authors
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System.Diagnostics;
using UI.Extensions;

namespace UI.GFX.Geometry;

public static class TransformUtil
{
    // Returns a scale transform at |anchor| point.
    public static Transform GetScaleTransform(in Point anchor, float scale)
    {
        Transform transform = new();
        transform.Translate(anchor.X * (1 - scale), anchor.Y * (1 - scale));
        transform.Scale(scale, scale);
        return transform;
    }

    // Interpolates the decomposed components |to| with |from| using the
    // routines described in
    // https://www.w3.org/TR/css-transforms-2/#interpolation-of-decomposed-3d-matrix-values
    // |progress| is in the range [0, 1]. If 0 we will return |from|, if 1, we will return |to|.
    public static DecomposedTransform BlendDecomposedTransforms(in DecomposedTransform to, in DecomposedTransform from, double progress)
    {
        DecomposedTransform out_ = new();
        double scalea = progress;
        double scaleb = 1.0 - progress;

        out_.Translate.X = to.Translate.X * scalea + from.Translate.X * scaleb;
        out_.Translate.Y = to.Translate.Y * scalea + from.Translate.Y * scaleb;
        out_.Translate.Z = to.Translate.Z * scalea + from.Translate.Z * scaleb;

        out_.Scale.X = to.Scale.X * scalea + from.Scale.X * scaleb;
        out_.Scale.Y = to.Scale.Y * scalea + from.Scale.Y * scaleb;
        out_.Scale.Z = to.Scale.Z * scalea + from.Scale.Z * scaleb;

        out_.Skew.X = to.Skew.X * scalea + from.Skew.X * scaleb;
        out_.Skew.Y = to.Skew.Y * scalea + from.Skew.Y * scaleb;
        out_.Skew.Z = to.Skew.Z * scalea + from.Skew.Z * scaleb;

        out_.Perspective.X = to.Perspective.X * scalea + from.Perspective.X * scaleb;
        out_.Perspective.Y = to.Perspective.Y * scalea + from.Perspective.Y * scaleb;
        out_.Perspective.Z = to.Perspective.Z * scalea + from.Perspective.Z * scaleb;
        out_.Perspective.W = to.Perspective.W * scalea + from.Perspective.W * scaleb;

        out_.Quaternion = from.Quaternion.Slerp(to.Quaternion, progress);
        return out_;
    }

    // Accumulates the decomposed components |to| with |from| using the routines described in
    // https://www.w3.org/TR/css-transforms-2/#combining-transform-lists
    public static DecomposedTransform AccumulateDecomposedTransforms(in DecomposedTransform a, in DecomposedTransform b)
    {
        DecomposedTransform out_ = new();

        // Translate is a simple addition.
        out_.Translate.X = a.Translate.X + b.Translate.X;
        out_.Translate.Y = a.Translate.Y + b.Translate.Y;
        out_.Translate.Z = a.Translate.Z + b.Translate.Z;

        // Scale is accumulated using 1-based addition.
        out_.Scale.X = a.Scale.X + b.Scale.X - 1;
        out_.Scale.Y = a.Scale.Y + b.Scale.Y - 1;
        out_.Scale.Z = a.Scale.Z + b.Scale.Z - 1;

        // Skew can be added.
        out_.Skew.X = a.Skew.X + b.Skew.X;
        out_.Skew.Y = a.Skew.Y + b.Skew.Y;
        out_.Skew.Z = a.Skew.Z + b.Skew.Z;

        // We sum the perspective components; note that w is 1-based.
        out_.Perspective.X = a.Perspective.X + b.Perspective.X;
        out_.Perspective.Y = a.Perspective.Y + b.Perspective.Y;
        out_.Perspective.Z = a.Perspective.Z + b.Perspective.Z;
        out_.Perspective.W = a.Perspective.W + b.Perspective.W - 1;

        // To accumulate quaternions, we multiply them. This is equivalent to
        // 'adding' the rotations that they represent.
        out_.Quaternion = a.Quaternion * b.Quaternion;

        return out_;
    }

    // Calculates a transform with a transformed origin. The resulting transform
    // is created by composing P * T * P^-1 where P is a constant transform to
    // the new origin.
    public static Transform TransformAboutPivot(in PointF pivot, in Transform transform)
    {
        Transform result = new();
        result.Translate(pivot.X, pivot.Y);
        result.PreConcat(transform);
        result.Translate(-pivot.X, -pivot.Y);
        return result;
    }

    // Calculates a transform which would transform |src| to |dst|.
    public static Transform TransformBetweenRects(in RectF src, in RectF dst)
    {
#if DEBUG
        Debug.Assert(!src.IsEmpty());
#endif
        Transform result = new();
        result.Translate(dst.Origin - src.Origin);
        result.Scale(dst.Width / src.Width, dst.Height / src.Height);
        return result;
    }

    // Returns the 2d axis transform that maps the clipping frustum to the square
    // from [-1, -1] (the original bottom-left corner) to [1, 1] (the original
    // top-right corner).
    public static AxisTransform2D OrthoProjectionTransform(float left, float right, float bottom, float top)
    {
        float delta_x = right - left;
        float delta_y = top - bottom;

        if (delta_x == 0 || delta_y == 0)
            return new AxisTransform2D();

        return AxisTransform2D.FromScaleAndTranslation(
            new Vector2DF(2.0f / delta_x, 2.0f / delta_y),
            new Vector2DF(-(right + left) / delta_x, -(top + bottom) / delta_y));
    }

    // Returns the 2d axis transform that maps from ([-1, -1] .. [1, 1]) to
    // ([x, y] .. [x + width, y + height]).
    public static AxisTransform2D WindowTransform(int x, int y, int width, int height)
    {
        return AxisTransform2D.FromScaleAndTranslation(
            new Vector2DF(width * 0.5f, height * 0.5f),
            new Vector2DF(x + width * 0.5f, y + height * 0.5f));
    }

    private static bool NearlyZero(double value) => Math.Abs(value) < double.MachineEpsilon;

    private static float ScaleOnAxis(double a, double b, double c)
    {
        if (NearlyZero(b) && NearlyZero(c))
            return ClampFloatGeometryHelper.ClampFloatGeometry(Math.Abs(a));
        if (NearlyZero(a) && NearlyZero(c))
            return ClampFloatGeometryHelper.ClampFloatGeometry(Math.Abs(b));
        if (NearlyZero(a) && NearlyZero(b))
            return ClampFloatGeometryHelper.ClampFloatGeometry(Math.Abs(c));

        // Do the sqrt as a double to not lose precision.
        return ClampFloatGeometryHelper.ClampFloatGeometry(Math.Sqrt(a * a + b * b + c * c));
    }

    // Compute 2D scale if possible, clamped with ClampFloatGeometry().
    public static Vector2DF? TryComputeTransform2dScaleComponents(in Transform transform)
    {
        if (transform.rc(3, 0) != 0.0 || transform.rc(3, 1) != 0.0)
            return null;

        double w = transform.rc(3, 3);
        if (!double.IsNormal(w))
            return null;

        float w_scale = 1.0f / (float)w;

        float x_scale = ScaleOnAxis(transform.rc(0, 0), transform.rc(1, 0), transform.rc(2, 0));
        float y_scale = ScaleOnAxis(transform.rc(0, 1), transform.rc(1, 1), transform.rc(2, 1));

        return new Vector2DF(
            ClampFloatGeometryHelper.ClampFloatGeometry(x_scale * w_scale),
            ClampFloatGeometryHelper.ClampFloatGeometry(y_scale * w_scale));
    }

    // Compute 2D scale, and fall back to fallback_value if not possible.
    public static Vector2DF ComputeTransform2dScaleComponents(
        in Transform transform, float fallback_value)
    {
        Vector2DF? scale = TryComputeTransform2dScaleComponents(transform);
        return scale ?? new Vector2DF(fallback_value, fallback_value);
    }

    // Returns an approximate max scale value of the transform even if it has
    // perspective. Prefer to use ComputeTransform2dScaleComponents if there is
    // no perspective, since it can produce more accurate results.
    public static float ComputeApproximateMaxScale(in Transform transform)
    {
        RectF unit = transform.MapRect(new RectF(0f, 0f, 1f, 1f));
        return Math.Max(unit.Width, unit.Height);
    }
}
