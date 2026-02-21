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

        Matrix44 matrix1 = new ();
        Matrix44 matrix2 = new ();

        matrix1[0, 0] = 2;
        
        Debug.WriteLine($"matrix1 == matrix2: {matrix1 == matrix2}");
        Debug.WriteLine($"matrix1 0,0: {matrix1[0, 0]}");
        Debug.WriteLine($"matrix2 0,0: {matrix2[0, 0]}");
    }
}
