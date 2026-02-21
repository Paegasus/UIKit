using UI.Geometry;
using SkiaSharp;

using UI.Tests;
using UI.GFX.Geometry;
using System.Diagnostics;

namespace UI;

public static class Program
{
    private static void RunTests()
    {
        LayoutUnitTest.Run();
        Vector2DTest.Run();
        Vector2DFTest.Run();
        Vector3DFTest.Run();
        AxisTransform2DTest.Run();
    }

    public static void Main(string[] args)
    {
        //RunTests()

        Matrix44 matrix = new ();

        Debug.WriteLine($"matrix1 IsIdentity: {matrix.IsIdentity}");
        Debug.WriteLine($"matrix1 IsIdentityOrTranslation: {matrix.IsIdentityOrTranslation}");
        Debug.WriteLine($"matrix1 IsScaleOrTranslation: {matrix.IsScaleOrTranslation}");
        Debug.WriteLine($"matrix1 IsScale: {matrix.IsScale}");
        Debug.WriteLine($"matrix1 IsFlat: {matrix.IsFlat}");
        Debug.WriteLine($"matrix1 HasPerspective: {matrix.HasPerspective}");
        Debug.WriteLine($"matrix1 Is2dTransform: {matrix.Is2DTransform}");
    }
}
