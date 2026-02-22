using UI.Geometry;
using SkiaSharp;

using UI.GFX.Geometry;
using System.Diagnostics;

namespace UI;

public static class Program
{
    public static void Main(string[] args)
    {
        Matrix44 matrix = new ();

        Debug.WriteLine($"matrix1 IsIdentity: {matrix.IsIdentity}");
        Debug.WriteLine($"matrix1 IsIdentityOrTranslation: {matrix.IsIdentityOrTranslation}");
        Debug.WriteLine($"matrix1 IsScaleOrTranslation: {matrix.IsScaleOrTranslation}");
        Debug.WriteLine($"matrix1 IsScale: {matrix.IsScale}");
        Debug.WriteLine($"matrix1 IsFlat: {matrix.IsFlat}");
        Debug.WriteLine($"matrix1 HasPerspective: {matrix.HasPerspective}");
        Debug.WriteLine($"matrix1 Is2DTransform: {matrix.Is2DTransform}");
    }
}
