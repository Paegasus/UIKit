using UI.Geometry;
using SkiaSharp;

using UI.GFX.Geometry;
using System.Diagnostics;

namespace UI;

public static class Program
{
    public static void Main(string[] args)
    {
        /*
        Quaternion q1 = new Quaternion(new Vector3DF(0, 0, 0), 1);
        Quaternion q2 = new Quaternion(new Vector3DF(1, 0, 0), 1);

        Transform t1 = new Transform(q1);
        Transform t2 = new Transform(q2);

        if(t1 == t2)
        {
            Console.WriteLine("Transforms are equal.");
        }
        else
        {
            Console.WriteLine("Transforms are NOT equal.");
        }
        */

        /*
        Matrix44 matrix = new ();

        Debug.WriteLine($"matrix1 IsIdentity: {matrix.IsIdentity}");
        Debug.WriteLine($"matrix1 IsIdentityOrTranslation: {matrix.IsIdentityOrTranslation}");
        Debug.WriteLine($"matrix1 IsScaleOrTranslation: {matrix.IsScaleOrTranslation}");
        Debug.WriteLine($"matrix1 IsScale: {matrix.IsScale}");
        Debug.WriteLine($"matrix1 IsFlat: {matrix.IsFlat}");
        Debug.WriteLine($"matrix1 HasPerspective: {matrix.HasPerspective}");
        Debug.WriteLine($"matrix1 Is2DTransform: {matrix.Is2DTransform}");
        */
    }
}
