using System.Runtime.CompilerServices;

namespace UI.GFX.Geometry;

public static class TransformUtil
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Combine(ref Vector3D out_, in Vector3D a, in Vector3D b, double scale_a, double scale_b)
    {
        out_.X = a.X * scale_a + b.X * scale_b;
        out_.Y = a.Y * scale_a + b.Y * scale_b;
        out_.Z = a.Z * scale_a + b.Z * scale_b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Combine(ref Vector4D out_, in Vector4D a, in Vector4D b, double scale_a, double scale_b)
    {
        out_.X = a.X * scale_a + b.X * scale_b;
        out_.Y = a.Y * scale_a + b.Y * scale_b;
        out_.Z = a.Z * scale_a + b.Z * scale_b;
        out_.W = a.W * scale_a + b.W * scale_b;
    }

    public static DecomposedTransform BlendDecomposedTransforms(in DecomposedTransform to, in DecomposedTransform from, double progress)
    {
        DecomposedTransform out_ = new();
        double scalea = progress;
        double scaleb = 1.0 - progress;
        Combine(ref out_.Translate, to.Translate, from.Translate, scalea, scaleb);
        Combine(ref out_.Scale, to.Scale, from.Scale, scalea, scaleb);
        Combine(ref out_.Skew, to.Skew, from.Skew, scalea, scaleb);
        Combine(ref out_.Perspective, to.Perspective, from.Perspective, scalea, scaleb);
        out_.Quaternion = from.Quaternion.Slerp(to.Quaternion, progress);
        return out_;
    }
}
